using System.Collections.Generic;
using UnityEngine;

public class NewsPopupCom : MonoBehaviour
{
    private List<NewsUI> activeNews = new List<NewsUI>();

    [SerializeField] private SerializableDictionary<NewsType, float> newsDisplayDuration = new();
    [SerializeField] private int maxNewsOnScreen = 5; // 最大同时显示的新闻数量
    [SerializeField] private GameObject newsUIPrefab;
    [SerializeField] private float newsSpacing = 10f; // 新闻之间的间距

    [SerializeField] private float newsFadeInDuration = 0.5f;
    [SerializeField] private float newsFadeOutDuration = 0.3f;
    [SerializeField] private float newsSlideDistance = 100f; // 滑入距离
    public void PopupNews(PopupNewsArgs args)
    {

        float duration = newsDisplayDuration[args.newsType];

        // 检查是否需要移除旧新闻
        if (activeNews.Count >= maxNewsOnScreen)
        {
            NewsUI oldNews = activeNews[0];
            activeNews.RemoveAt(0);
            oldNews.HideNews();
        }

        NewsUI newsUI = newsUIPrefab.OPGet(transform).GetComponent<NewsUI>();

        // 先添加到活动列表
        activeNews.Add(newsUI);

        // 重新排列所有新闻（设置目标位置）
        RearrangeNews();

        // 然后显示新闻
        newsUI.ShowNews(args.title, args.content, args.newsType, duration, newsFadeInDuration, newsFadeOutDuration, newsSlideDistance);


    }
    public void UpdateHistory()
    {

    }
    /// <summary>
    /// 重新排列所有新闻
    /// </summary>
    private void RearrangeNews()
    {
        for (int i = 0; i < activeNews.Count; i++)
        {
            if (activeNews[i] != null)
            {
                Vector3 targetPosition = CalculateNewsPosition(i);
                activeNews[i].SetTargetPosition(targetPosition);
                Debug.Log($"新闻 {i} 位置设置为: {targetPosition}");
            }
        }
    }
    /// <summary>
    /// 计算新闻位置
    /// </summary>
    private Vector3 CalculateNewsPosition(int index)
    {
        // 获取新闻的实际高度
        float newsHeight = GetNewsHeight();

        // 从右上角开始，向下排列
        float yOffset = -index * (newsHeight + newsSpacing);
        return new Vector3(0, yOffset, 0);
    }
    /// <summary>
    /// 获取新闻高度
    /// </summary>
    private float GetNewsHeight()
    {
        if (newsUIPrefab != null)
        {
            RectTransform rectTransform = newsUIPrefab.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                return rectTransform.rect.height * rectTransform.localScale.y;
            }
        }

        // 默认高度
        return 80f;
    }

    /// <summary>
    /// 清除所有新闻
    /// </summary>
    public void Clear()
    {
        foreach (NewsUI news in activeNews)
        {
            if (news != null)
            {
                news.HideNews();
                news.gameObject.OPPush();
            }
        }
        activeNews.Clear();
        newsUIPrefab.OPClear();
    }


}