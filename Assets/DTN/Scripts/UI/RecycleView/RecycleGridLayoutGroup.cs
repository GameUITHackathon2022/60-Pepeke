using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.EventSystems;


namespace Cooldhands.UI
{
		
	/// <summary>
	/// It is responsible for reusing the objects that left the screen while scrolling,
	/// GridLayoutGroup component that can handle a large number of lists without degrading performance
	/// </summary>
	public class RecycleGridLayoutGroup : MonoBehaviour
	{
		public enum SnapToEnum { None, Item, Page }
		//****************************************************************
		#region Variables

		/// Item prefab
		[Header("Item")]
		[SerializeField] private GameObject _listItemPrefab = null;
		[SerializeField] private Vector2 _cellSpacing = new Vector2( 0, 0 );

		/// List item size
		private Vector2 _cellSize = new Vector2( 32, 32 );

		[Header("Grid")]
		[SerializeField] private int _rows = 1;
		[SerializeField] private int _columns = 1;
		[SerializeField] private float _nextSizePreview = 30;
		///Total number of items in the list
		private int _listItemCount = 0;
		private int _bufferLength = 1;

		[Header("Datasource")]
		[SerializeField] private UnityEngine.Object _datasource = null;

		[Header("Snap To")]
		[SerializeField] private SnapToEnum _snapTo = SnapToEnum.None;

		/// The displayed object dictionary
		private Dictionary<int,ListItemContent> _showObjectDic = new Dictionary<int, ListItemContent>();
		
		/// Hidden Object List. There is no need to identify it, so this is just an array
		private List<ListItemContent> _hideObjects = new List<ListItemContent>();

		/// Transform to store hidden objects
		private Transform _recycleBoxObject = null;
		/// Number of items that can be displayed in a column
		private int _lineCount = 1;
		/// Initialization flag
		private bool _isInit = false;

		/// Event that fires immediately after the list item becomes visible
		private Action<ListItemContent,Action> _onVisibleListContent;
		/// Event that fires immediately after the list item is hidden
		private Action<ListItemContent> _onHideListContent;

		/// Periodic cleaning of hidden objects.
		private Coroutine _cleaningCoroutine = null;

		private ScrollRect _scrollRect;

		private Coroutine _scrollCoroutine;

		private Vector3 _scrollFromPosition, _scrollToPosition;

		private bool _autoScroll;

		private Vector3 _lastScrollPosition;

		private Coroutine _updateSizeCoroutine = null;

		#endregion


		//****************************************************************
		#region Properties

		/// Facilitates access to rectTransform
		private RectTransform _rectTransform = null;
		private RectTransform rectTransform {
			get {
				if( _rectTransform == null ) _rectTransform = this.transform as RectTransform;
				return _rectTransform;
			}
		}

		/// Total number of list items
		private int listItemCount {
			get { return _listItemCount; }
			set { _listItemCount = value; CalcScrollArea(); }
		}

		/// Is an autoscroll being performed?
		public bool isAutoScrolling
		{
			get{
				return _autoScroll;
			}
		}

		/// How many additional columns to keep for recycling (1 = 1 column)
		public int bufferLength {
			get { return _bufferLength; }
			set { _bufferLength = value; }
		}

		/// Event that fires immediately after the list item becomes visible
		public Action<ListItemContent,Action> onVisibleListItem {
			get { return _onVisibleListContent; }
			set { _onVisibleListContent = value; }
		}

		/// Event that fires immediately after the list item is hidden
		public Action<ListItemContent> onHideListItem {
			get { return _onHideListContent; }
			set { _onHideListContent = value; }
		}

		/// Total number of items in the list
		private int lineCount {
			get { return _lineCount; }
			set { _lineCount = (value <= 0) ? 1 : value; }
		}

		/// List item size
		public Vector2 cellSize {
			get { return _cellSize; }
			//set { _cellSize = value; }
		}

		/// Space between items
		public Vector2 cellSpacing {
			get { return _cellSpacing; }
			set { _cellSpacing = value; }
		}

		/// Is it a horizontal scroll?
		public bool horizontal
		{
			get{
				return _scrollRect.horizontal;
			}
		}

		/// Is it a vertical scroll?
		public bool vertical
		{
			get{
				return _scrollRect.vertical;
			}
		}

		#endregion

		//****************************************************************
		#region MonoBehaviour LifeCycle

		/// <summary>
		/// Start is called on the frame when a script is enabled just before
		/// any of the Update methods is called the first time.
		/// </summary>
		void Start() {
			StartCoroutine(InitScrollArea());
		}

		/// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
		void Awake()
		{
			_scrollRect = rectTransform.GetComponentInParent<ScrollRect>();
			if(_scrollRect == null)
			{
				throw new Exception("RecycleGridLayoutGroup: ScrollRect is required in the parent.");
			}

			if(this.vertical && this.horizontal)
			{
				throw new Exception("RecycleGridLayoutGroup: Only horizontal or vertical scrolling in ScrollRect is supported, not both.");
			}

			SetDatasource();
			SetSnapTo();

			if(this.vertical)
			{
				rectTransform.anchorMin = new Vector2(0, 1);
				rectTransform.anchorMax = new Vector2(1, 1);
			}
			else{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(0, 1);
			}
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		void OnEnable() {
			StartCleaningCoroutine();
		}

		/// <summary>
		/// This function is called when the behaviour becomes disabled or inactive.
		/// </summary>
		void OnDisable() {
			StopCleaningCoroutine();
		}
        void OnRectTransformDimensionsChange()
		{
			if(_isInit && this.isActiveAndEnabled && _updateSizeCoroutine == null)
			{
				_updateSizeCoroutine = StartCoroutine(UpdateSizeCoroutine());
			}
		}

		#endregion

		#region Snap

		private void SetSnapTo()
		{
			ScrollRectEvents sRectEvents = _scrollRect.GetComponent<ScrollRectEvents>();

			if(_snapTo != SnapToEnum.None)
			{
				if(sRectEvents == null)
				{
					sRectEvents = _scrollRect.gameObject.AddComponent<ScrollRectEvents>();
				}
				sRectEvents.RecycleGridLayout = this;
				sRectEvents.ScrollRect = _scrollRect;
				sRectEvents.EnableSnap();
			}
			else{
				if(sRectEvents != null)
				{
					sRectEvents.DisableSnap();
				}
			}
		}

		#endregion

		//****************************************************************
		#region Calculate the scroll area

		/// <summary>
		/// Calculate the scroll area
		/// </summary>
		private void CalcScrollArea()
		{
			if(!_isInit)
				return;
			Vector2 listItemInterval = new Vector2( cellSize.x + cellSpacing.x, cellSize.y + cellSpacing.y );

			Vector2 scrollArea = Vector3.zero;
			if(this.vertical)
				scrollArea = new Vector2( 0, listItemInterval.y );
			else
				scrollArea = new Vector2( listItemInterval.x, 0 );

			if( (_listItemCount % lineCount) > 0 ) {
				if(this.vertical)
					scrollArea.y *= ((_listItemCount / lineCount) + 1);
				else
				{
					scrollArea.x *= ((_listItemCount / lineCount) + 1);					
				}
			}
			else {
				if(this.vertical)
					scrollArea.y *= (_listItemCount / lineCount);
				else
				{
					scrollArea.x *= ((_listItemCount / lineCount));
				}
			}

			rectTransform.sizeDelta = scrollArea;

			OnMoveScrollEvent( Vector2.zero );
		}

		private IEnumerator UpdateSizeCoroutine()
		{
			yield return null;
			int currentIndex = GetCurrentIndex();
			Vector2 currectCellSize = new Vector2(this.cellSize.x, this.cellSize.y);
			CalcCellSize();
			if(currectCellSize.x != this.cellSize.x || currectCellSize.y != this.cellSize.y)
			{
				CalcScrollArea();
				UpdateListAll();
			}
			_updateSizeCoroutine = null;
		}

		/// <summary>
		/// Initialize the scroll area
		/// </summary>
		private IEnumerator InitScrollArea()
		{
			_isInit = false;

			// A place to temporarily save objects that have left the screen
			if( _recycleBoxObject == null )
			{
				GameObject go = new GameObject();
				go.name = "RecycleContent";
				_recycleBoxObject = go.transform;
				_recycleBoxObject.SetParent( rectTransform.parent );
				_recycleBoxObject.localPosition = Vector3.zero;
				_recycleBoxObject.localScale = Vector3.one;
			}

			if( _scrollRect != null ) {
				_scrollRect.onValueChanged.AddListener(OnMoveScrollEvent);
			}
			//Delete the lists
			_showObjectDic = new Dictionary<int, ListItemContent>();
			_hideObjects = new List<ListItemContent>();
			foreach( Transform tf in this.transform ) {
				Destroy( tf.gameObject );
			}
			foreach( Transform tf in _recycleBoxObject ) {
				Destroy( tf.gameObject );
			}

			// Before the update (just after the Start)
			// Somehow the width of the rect can't be taken well
			yield return null;

			CalcCellSize();

			_isInit = true;

			yield return null;

			CalcScrollArea();
		}

		private void CalcCellSize()
		{
			var viewPort = (rectTransform.parent as RectTransform);
			var rect = viewPort.rect;

			if(this.vertical)
				this._cellSize = new Vector2((int)(((rect.width) / _columns)-cellSpacing.x), (int)(((rect.height-((_nextSizePreview*2) + cellSpacing.y))/_rows) - cellSpacing.y));
			else
				this._cellSize = new Vector2((int)(((rect.width-((_nextSizePreview*2) + cellSpacing.x)) / _columns)-cellSpacing.x), (int)((rect.height / _rows) - cellSpacing.y));

			Vector2 listItemInterval = new Vector2( cellSize.x + cellSpacing.x, cellSize.y + cellSpacing.y );

			if(this.vertical)
				lineCount = (int)( rect.width / listItemInterval.x );
			else
				lineCount = (int)( rect.height / listItemInterval.y );
		}

		#endregion

		//****************************************************************
		#region Processing during scroll event

		/// <summary>
		/// Processing during scroll event
		/// </summary>
		/// <param name="scrollPos">Scroll position.</param>
		public void OnMoveScrollEvent(Vector2 scrollPos)
		{
			if( _isInit == false || _listItemCount == 0 ) return;

			float viewTop = GetViewTop();
			float viewBottom = GetViewBottom();
			float viewLeft = GetViewLeft();
			float viewRight = GetViewRight();

			viewBottom -= (cellSize.y + cellSpacing.y);
			viewTop += (cellSize.y + cellSpacing.y);

			viewRight += (cellSize.x + cellSpacing.x);
			viewLeft -= (cellSize.x + cellSpacing.x);

			// Hides the items in the list outside the display area
			HideObjectsOutsideView( viewTop, viewBottom, viewLeft, viewRight );

			// Show the list items within the display area
			ShowObjectsInsideView( viewTop, viewBottom, viewLeft, viewRight  );
		}

		private float GetViewLeft(bool withNextSize = false)
		{
			return (rectTransform.localPosition.x - (withNextSize ? (_nextSizePreview + cellSpacing.x) : 0)) *-1;
		}

		private float GetViewRight()
		{
			float viewLeft = GetViewLeft() * -1;
			RectTransform viewRl = this.transform.parent as RectTransform;
			return (viewRl.rect.width - viewLeft);
		}

		private float GetViewTop(bool withNextSize = false)
		{
			float top = (rectTransform.localPosition.y + (withNextSize ? (_nextSizePreview + cellSpacing.y) : 0)) * -1;
			return top;
		}

		private float GetViewBottom()
		{
			float viewTop = GetViewTop();
			RectTransform viewRt = this.transform.parent as RectTransform;
			return viewTop - viewRt.rect.height;
		}

		//Indicates if the item is fully visible
		private bool IsItemFullyVisible(int index, float marginError = 0.1f)
		{
			bool result = false;
			if( _showObjectDic != null && _showObjectDic.ContainsKey(index)) {
				var item = _showObjectDic[index];
				float viewTop = Mathf.Ceil(GetViewTop()) + marginError;
				float viewBottom = GetViewBottom() - marginError;

				float viewLeft = Mathf.Floor(GetViewLeft()) - marginError;
				float viewRight = GetViewRight() + marginError;

				if(this.vertical)
				{
					float itemTop = item.rectTransform.localPosition.y;
					float itemBottom = itemTop - cellSize.y - cellSpacing.y;
					if(itemTop <= viewTop  && itemBottom >= viewBottom)
					{
						result = true;
					}
				}
				else
				{
					float itemLeft = item.rectTransform.localPosition.x;
					float itemRight = itemLeft + cellSize.x + cellSpacing.x;

					if(itemLeft >= viewLeft  && itemRight <= viewRight)
					{
						result = true;
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Hides the items in the list outside the display area
		/// </summary>
		/// <param name="viewTop">View top.</param>
		/// <param name="viewBottom">View bottom.</param>
		/// <param name="viewLeft">View left.</param>
		/// <param name="viewRight">View right.</param>
		private void HideObjectsOutsideView( float viewTop, float viewBottom, float viewLeft, float viewRight)
		{
			//var allChildren = rectTransform.GetComponentsInChildren<ListItemContent>();
			var allChildren = _showObjectDic.Select(a=>a.Value).ToArray();
			
			foreach(ListItemContent child in allChildren)
			{
				if( child.gameObject.activeSelf == false ) continue;

				RectTransform rt = child.transform as RectTransform;

				float itemTop = rt.localPosition.y;
				float itemBottom = itemTop - cellSize.y - cellSpacing.y;
				float itemLeft = rt.localPosition.x;
				float itemRight = itemLeft + cellSize.x + cellSpacing.x;

				if(this.vertical)
				{
					if(itemBottom > viewTop || itemTop < viewBottom) {
						HideListItem(child);
					}
				}
				else{
					if(itemRight < viewLeft || itemLeft > viewRight) {
						HideListItem(child);
					}
				}
			}
		}

		/// <summary>
		/// Show the list items within the display area
		/// </summary>
		/// <param name="viewTop">View top.</param>
		/// <param name="viewBottom">View bottom.</param>
		/// <param name="viewLeft">View left.</param>
		/// <param name="viewRight">View right.</param>
		private void ShowObjectsInsideView( float viewTop, float viewBottom, float viewLeft, float viewRight)
		{
			int i = GetCurrentIndex();

			if(this.vertical)
			{
				if(i != 0)
				{
					i = (int)(Mathf.Abs( viewTop ) / (cellSize.y + cellSpacing.y)) * (lineCount);
				}
			}
			else{
				if(i != 0)
				{
					i = (int)(Mathf.Abs( viewLeft ) / (cellSize.x + cellSpacing.x)) * (lineCount);
				}
			}
			i--;

			while(true)
			{
				i++;
				if( i >= _listItemCount ) break;

				if(this.vertical)
				{
					float top = GetRowsPosByIndex(i);
					if( top < viewBottom ) break;
				}
				else{
					float left = GetColsPosByIndex(i);
					if( left > viewRight) break;
				}
				if( _showObjectDic.ContainsKey(i)) { continue; }

				RecycleListItem(i);
			}
		}

		#endregion

		#region All updates

		/// <summary>
		/// Update all lists
		/// (only the visible list is updated)
		/// </summary>
		public void UpdateListAll()
		{
			List<ListItemContent> showObj = new List<ListItemContent>();
			foreach( ListItemContent item in _showObjectDic.Values ) {
				showObj.Add( item );
			}

			// First, hide everything
			foreach( ListItemContent item in showObj ) {
				HideListItem( item );
			}

			// Update visible item
			if( _isInit == false || _listItemCount == 0 ) return;

			float viewTop = GetViewTop();
			float viewBottom = GetViewBottom();
			float viewLeft = GetViewLeft ();
			float viewRight = GetViewRight();

			ShowObjectsInsideView( viewTop, viewBottom, viewLeft, viewRight );
		}

		#endregion

		//****************************************************************
		#region Show / hide list items

		/// <summary>
		/// Hide list item
		/// </summary>
		/// <param name="listItem">List item.</param>
		private void HideListItem( ListItemContent listItem )
		{
			_showObjectDic.Remove( listItem.index );
			_hideObjects.Add( listItem );
			
			listItem.gameObject.SetActive( false );
			listItem.transform.SetParent( _recycleBoxObject );
			listItem.transform.localPosition = Vector3.zero;
			listItem.isHide = true;

			if( _onHideListContent != null ) _onHideListContent( listItem );
		}

		/// <summary>
		/// Recycle any hidden object that can be recycled,
		/// If not, create a new one
		/// </summary>
		/// <returns>The list item.</returns>
		/// <param name="index">Index.</param>
		private ListItemContent RecycleListItem( int index )
		{
			if( index >= _listItemCount ) return null;

			GameObject obj;
			ListItemContent listItem;
			RectTransform rt;

			
			// Is it display?
			if( _showObjectDic != null && _showObjectDic.ContainsKey( index ) ) {
				return _showObjectDic[index];
			}
			// If it is not yet displayed
			else
			{
				listItem = null;

				// Find recyclable items
				foreach( ListItemContent hideObj in _hideObjects ) {
					if( hideObj.state == ListItemContent.eState.Ready ) {
						listItem = hideObj;
						break;
					}
				}

				// Reuse any recyclable item
				if( listItem != null ) {
					obj = listItem.gameObject;
					obj.SetActive( true );
					obj.transform.SetParent( this.transform );
					//listItem = obj.GetComponent<ListItemContent>();
					_hideObjects.Remove( listItem );
					_showObjectDic.Add( index, listItem );
				}
				// New if there are no recyclables
				else {
					obj = Instantiate( _listItemPrefab );
					obj.transform.SetParent( this.transform );
					var rtf = obj.transform as RectTransform;
					rtf.pivot = new Vector2( 0.0f, 1.0f );

					listItem = obj.GetComponent<ListItemContent>();
					if(listItem == null)
					{
						listItem = obj.AddComponent<ListItemContent>();
					}
					listItem.recycleGridLayout = this;
					_showObjectDic.Add( index, listItem );
				}
			}

			// Calculate coordinates from the index number
			float x = GetColsPosByIndex( index );
			float y = GetRowsPosByIndex( index );

			// List item settings
			obj.name = index.ToString();
			listItem.index = index;
			listItem.isHide = false;
			listItem.state = ListItemContent.eState.Loading;
			// Correct position / size
			rt = obj.transform as RectTransform;
			rt.localPosition = new Vector3( x, y, 0.0f );
			rt.localScale = Vector3.one;
			rt.sizeDelta = new Vector2(cellSize.x, cellSize.y);

			if(_datasource != null)
			{
				listItem.Data = ((IDataSource)_datasource).GetData(listItem.index);
			}

			// Call again to notify that the display status has been reached
			if( _onVisibleListContent != null ) {
				_onVisibleListContent( listItem, () => {
					listItem.state = ListItemContent.eState.LoadCompleted;
				});
			}
			else {
				listItem.state = ListItemContent.eState.LoadCompleted;
			}

			IRecycleItemChanged itemEvent = listItem.gameObject.GetComponent<IRecycleItemChanged>();
			if(itemEvent != null)
			{
				itemEvent.OnRecycle(listItem, () => {
					listItem.state = ListItemContent.eState.LoadCompleted;
				});
			}			

			return listItem;
		}
		
		/// <summary>
		/// Remove
		/// </summary>
		/// <param name="listItem"></param>
		private void DestroyListItem( ListItemContent listItem )
		{
			if( _hideObjects.Count <= (bufferLength*lineCount) ) return;

			if( _showObjectDic.ContainsKey( listItem.index ) ) {
				_showObjectDic.Remove(listItem.index);
			}
			_hideObjects.Remove( listItem );
			GameObject.Destroy( listItem.gameObject );
		}

		#endregion


		//****************************************************************
		#region Cleaning coroutine

		/// <summary>
		/// Start a routine that removes useless objects
		/// </summary>
		public void StartCleaningCoroutine()
		{
			if( _cleaningCoroutine != null ) StopCoroutine( _cleaningCoroutine );
			_cleaningCoroutine = StartCoroutine( ClockSignal( 1.0f, () => {
				OnCleaning( 0.1f );
			}));
		}

		/// <summary>
		/// Stop the routine that removes useless objects
		/// </summary>
		public void StopCleaningCoroutine()
		{
			if( _cleaningCoroutine != null ) StopCoroutine( _cleaningCoroutine );
			_cleaningCoroutine = null;
		}

		/// <summary>
		/// A routine that removes useless objects
		/// </summary>
		/// <param name="useCpu">CPU usage (1.0f = 100%)</param>
		public void OnCleaning( float useCpu )
		{
			if( _hideObjects.Count <= (bufferLength*lineCount) ) return;

			float startTime = Time.realtimeSinceStartup;
			float limitTime = startTime + (useCpu / 60.0f);
			
			int len = _hideObjects.Count;
			for( int i = len - 1; i >= 0; --i )
			{
				if( _hideObjects.Count <= (lineCount * bufferLength) ) break;
				if( Time.realtimeSinceStartup >= limitTime ) break;
				
				ListItemContent listItem = _hideObjects[i];
				if( listItem.state != ListItemContent.eState.Ready ) continue;

				_hideObjects.Remove( listItem );
				GameObject.Destroy( listItem.gameObject );
			}
		}

		/// <summary>
		/// A routine that periodically returns a callback after standing for a certain period of time.
		/// </summary>
		/// <returns>The signal.</returns>
		/// <param name="interval">Interval.</param>
		/// <param name="callback">Callback.</param>
		public IEnumerator ClockSignal( float interval, Action callback )
		{
			while( true )
			{
				yield return new WaitForSeconds( interval );
				if( callback != null ) callback();
			}
		}

		#endregion

		#region Autoscroll

		/// <summary>
		/// Scroll to the indicated list position.
		/// <param name="index">Index position.</param>
		/// </summary>
		public void ScrollToPosition(int index)
		{
			ScrollToPosition(index, true);
		}
		private void ScrollToPosition(int index, bool onlyNoVisible)
		{
			_scrollRect.StopMovement();
			if(index < 0 || index > _listItemCount - 1)
			{
				throw new IndexOutOfRangeException();
			}

			if(_snapTo == SnapToEnum.Page)
			{
				int currentIndex = GetCurrentIndex();

				if(Mathf.Floor(currentIndex/ lineCount) < Mathf.Floor(index / lineCount))
					index = (int)((GetPage(index) + 1) * (_rows * _columns)) - 1;
				else
					index = (int)((GetPage(index)) * (_rows * _columns));
					
			}

			if((!IsItemFullyVisible(index) && onlyNoVisible) || !onlyNoVisible)
			{
				float yPos = _rectTransform.localPosition.y;
				float xPos = _rectTransform.localPosition.x;

				int currentIndex = GetCurrentIndex();

				if(this.vertical)
				{
					yPos = (cellSize.y + cellSpacing.y);
					yPos *= (index / lineCount);
					yPos-=  (_nextSizePreview + cellSpacing.y);

					if(Mathf.Floor(currentIndex/ lineCount) < Mathf.Floor(index / lineCount))
					{
						yPos -= ((_rows -1) * ((cellSize.y + cellSpacing.y)));
					}
				}
				else
				{
					xPos = (cellSize.x + cellSpacing.x);
					xPos *= (index / lineCount) * -1;
					xPos +=  (_nextSizePreview + cellSpacing.x);

					if(Mathf.Floor(currentIndex/ lineCount) < Mathf.Floor(index / lineCount))
					{
						xPos += ((_columns - 1) * ((cellSize.x + cellSpacing.x)));
					}
				}

				if(!_autoScroll)
				{
					_autoScroll = true;
					_scrollFromPosition = _rectTransform.localPosition;
					_scrollToPosition = new Vector2(xPos, yPos);
					if(_scrollCoroutine == null)
					{
						_scrollCoroutine = StartCoroutine(AutoScrollCoroutine(0.2f));
					}
				}		
			}
		}

		//Is responsible for performing the autoscroll
		private IEnumerator AutoScrollCoroutine(float duration)
		{
			Selectable selectableGameObject = null;
			// looks strange but as long as you yield somewhere inside
			// the loop it simply means repeat the sequence forever
			// just like the Update method
			while(true)
			{
				if(_autoScroll)
				{
					if(EventSystem.current.currentSelectedGameObject != null)
					{
					    selectableGameObject = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
						if(selectableGameObject != null)
						{
							//wait until the scrolling is over
							selectableGameObject.navigation = new Navigation { mode = Navigation.Mode.None };
						}
					} 
					var passedTime = 0f;
					while(passedTime < duration && Vector3.Distance(_rectTransform.localPosition, _scrollToPosition) > 0.2f)
					{
						var lerpFactor = passedTime / duration;
						//var smoothedLerpFactor = Mathf.SmoothStep(0, 1, lerpFactor);

						_rectTransform.localPosition = Vector3.Lerp(_scrollFromPosition, _scrollToPosition, lerpFactor);

						passedTime += Mathf.Min(duration - passedTime, Time.unscaledDeltaTime);
						
						// reads like: "pause" here, render this frame and continue
						// from here in the next frame
						yield return null;
					}
					yield return null;
					_rectTransform.localPosition = _scrollToPosition;
					//yield return new WaitForSeconds(0.05f);
					if(selectableGameObject != null)
					{
						selectableGameObject.navigation = new Navigation { mode = Navigation.Mode.Automatic };
					}
					_autoScroll = false;
					_scrollRect.StopMovement();
				}
				else{
					yield return null;
				}
			}
		}

		public void OnEndDrag(bool swipeLeft, bool swipeUp)
		{
			int currentIndex = GetCurrentIndex();

			if(this.vertical)
			{
				if(!swipeUp)
					currentIndex += (_rows * _columns);
			}
			else{
				if(!swipeLeft)
					currentIndex += (_rows * _columns);
			}

			if(currentIndex < 0)
				currentIndex =	0;

			if(currentIndex >= listItemCount)
				currentIndex = listItemCount - 1;

			ScrollToPosition(currentIndex, false);
		}

		public void ScrollToNext()
		{
			int currentIndex = GetCurrentIndex();
			currentIndex += (_rows * _columns);
			if(currentIndex >= listItemCount)
				currentIndex = listItemCount - 1;
			ScrollToPosition(currentIndex, false);
		}

		public void ScrollToPrev()
		{
			int currentIndex = GetCurrentIndex();
			if(currentIndex < 0)
				currentIndex =	0;
			ScrollToPosition(currentIndex, false);
		}
		#endregion

		//****************************************************************
		#region etc

		private float GetColsPosByIndex( int index ) {
			if(this.vertical)
				return (cellSize.x + cellSpacing.x) * (index % lineCount);
			else
				return ((cellSize.x + cellSpacing.x)) * (index / lineCount);
		}
		private float GetRowsPosByIndex( int index ) {
			if(this.vertical)
				return ((cellSize.y + cellSpacing.y)) * (index / lineCount) * -1;
			else
				return (cellSize.y + cellSpacing.y) * (index % lineCount) * -1;
		}

		///Get the page for the indicated position
		private int GetPage(int currentIndex)
		{
			return (int)Mathf.Floor(currentIndex/ (_rows * _columns));
		}

		///Get current index based on scroll position
		private int GetCurrentIndex()
		{
			if(this.vertical)
			{
				return (int)(Mathf.Abs( GetViewTop(true)) / ((cellSize.y + cellSpacing.y))) * (lineCount);
			}
			else
			{
				return (int)(Mathf.Abs( GetViewLeft(true)) / ((cellSize.x + cellSpacing.x))) * (lineCount);
			}
		}

		private void SetDatasource()
		{
			if(_datasource != null)
			{
				if(!(_datasource is IDataSource))
				{
					throw new Exception("RecycleGridLayoutGroup: The data source must implement the IDataSource interface.");
				}

				listItemCount = ((IDataSource)_datasource).GetCount();
			}
		}

		public void SetDatasource(IDataSource data)
		{
			_datasource = data as UnityEngine.Object;
			SetDatasource();
		}

		#endregion
	}
}
