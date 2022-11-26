using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DTNView : DTNMono
{
    public static DTNView _instance;

    public List<DTNView> _subViews = new List<DTNView>();
    public DTNView parentView;
    public List<DTNView> _history = new List<DTNView>();
    public Transform content;

    private void Awake()
    {
        _instance = this;
    }

    [HideInInspector]
    public bool isInit = false;
    public virtual void Initialize()
    {
        content = transform;
        Init();
        isInit = true;
    }

    public void InitIfNeed()
    {
        if (!isInit)
        {
            Initialize();
        }
    }

    public abstract void Init();

    public virtual void Hide()
    {
        DTNPoolingGameManager.Instance.DestroyObject(this.gameObject);
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        _subViews = new List<DTNView>();
    }

    public virtual void Back()
    {
        DTNView lastView = LastView();
        if (lastView != null)
        {
            lastView?.Hide();
            _history.RemoveAt(_history.Count - 1);

        }
        else if (parentView != null)
        {
            parentView.Back();
        }
        else
        {
            Hide();
        }

    }

    public virtual int IndexOf(DTNView view)
    {
        for (int i = 0; i < _history.Count; i++)
        {
            if (_history[i] == view)
            {
                return i;
            }
        }
        return -1;
    }

    public virtual void Back(int index)
    {
        int i = _history.Count - 1;
        while (i >= index && i < _history.Count && i >= 0)
        {
            _history[i].Hide();
            _history.RemoveAt(i);
        }
    }

    public virtual T ShowSubView<T>() where T : class
    {
        DTNView initView = DTNInitView._instance.Init<T>(transform);

        return ShowSubView(initView) as T;
    }

    public virtual DTNView ShowSubView(System.Type type)
    {
        DTNView initView = DTNInitView._instance.Init(type, transform);

        return ShowSubView(initView);
    }

    public virtual DTNView ShowSubView(DTNView view)
    {
        view.transform.parent = null;
        return Push(view);
    }

    public virtual DTNView LastView()
    {
        if (_history.Count <= 0)
            return null;

        return _history[_history.Count - 1];
    }

    public virtual T GetView<T>() where T : class
    {
        return DTNInitView._instance.Init<T>(transform) as T;
    }

    public virtual DTNView Push(DTNView view)
    {
        _history.Add(view);

        DTNView initView = view;
        initView.transform.parent = transform;
        initView.transform.localScale = new Vector3(1, 1, 1);
        initView.transform.localPosition = Vector3.zero;
        initView.transform.localRotation = Quaternion.identity;
        initView.gameObject.SetActive(true);
        _subViews.Add(initView);
        initView.parentView = this;
        initView.InitIfNeed();
        initView.Show();

        return initView;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        parentView = null;
    }
}