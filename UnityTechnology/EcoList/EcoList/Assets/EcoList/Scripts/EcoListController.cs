using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class EcoListController : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_Content;
    [SerializeField]
    private RectTransform m_PoolItem;
    [SerializeField]
    private RectTransform m_ScrollView;
    private RectTransform[] m_PoolItems;
    [SerializeField]
    private bool m_IsVertical;
    [SerializeField]
    private bool m_IsLoop;

    private Action<RectTransform, int> m_CallbackChanged;
    private Vector2 m_ItemSize;

    private int m_DataCount;
    private int m_PoolItemCount;
    private int m_MoveIndex;

    public void Init(int dataCount, Action<RectTransform, int> callbackChanged)
    {
        m_CallbackChanged = callbackChanged;

        m_MoveIndex = 0;
        m_DataCount = dataCount;
        CalculatePoolCount();
        SetContentType();
        //m_ItemSize = m_PoolItems[0].sizeDelta;
        //m_PoolItemCount = m_PoolItems.Length;

        for (int i = 0; i < m_PoolItems.Length; i++)
        {
            SetPositionPoolItem(i, i);
            if (m_CallbackChanged != null) m_CallbackChanged(m_PoolItems[i], i);
        }
    }

    private void SetContentType() 
    {
        var scrollRect = m_ScrollView.GetComponent<ScrollRect>();
        if (m_IsLoop)
        {
            scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
        }
        else 
        {
            scrollRect.movementType = ScrollRect.MovementType.Elastic;
            if (m_IsVertical)
            {
                m_Content.sizeDelta = new Vector2(m_Content.sizeDelta.x, GetItemSize() * m_DataCount);
            }
            else 
            {
                m_Content.sizeDelta = new Vector2(GetItemSize() * m_DataCount, m_Content.sizeDelta.y);
            }
        }
    }

    private void CalculatePoolCount()
    {
        if (null != m_PoolItem) 
        {
            m_ItemSize = m_PoolItem.sizeDelta;
            float svSize = m_IsVertical ? m_ScrollView.sizeDelta.y : m_ScrollView.sizeDelta.x;
            float itemSize = GetItemSize();
            m_PoolItemCount = Mathf.CeilToInt(svSize / itemSize) + 2;
            if (m_PoolItemCount > 0)
            {
                m_PoolItems = new RectTransform[m_PoolItemCount];
                for (int i = 0; i < m_PoolItemCount; i++)
                {
                    m_PoolItems[i] = Instantiate(m_PoolItem);
                    m_PoolItems[i].SetParent(m_Content, false);
                }
            }
        }
    } 

    void Update()
    {
        if (null == m_PoolItem) return;
        if (m_PoolItemCount == 0) return;

        float itemSize = GetItemSize();
        float contentPosition = GetContentPosition();

        while (itemSize * (m_MoveIndex + 2) < contentPosition)
        {
            int poolIndex = m_MoveIndex % m_PoolItemCount;
            int changedIndex = m_PoolItemCount + m_MoveIndex;
            int dataIndex = changedIndex % m_DataCount;

            if (m_MoveIndex < 0)
            {
                poolIndex = GetReverseIndex(m_MoveIndex, m_PoolItemCount);
                dataIndex = GetReverseIndex(m_MoveIndex - (m_DataCount - m_PoolItemCount), m_DataCount);
            }

            SetPositionPoolItem(poolIndex, changedIndex);
            m_MoveIndex++;
            //Debug.Log("1 = " + m_MoveIndex);

            if (m_CallbackChanged != null)
            {
                m_CallbackChanged(m_PoolItems[poolIndex], dataIndex);
            }
        }

        while (itemSize * (m_MoveIndex + 1) > contentPosition)
        {
            m_MoveIndex--;

            int poolIndex = m_MoveIndex % m_PoolItemCount;
            int changedIndex = m_MoveIndex;
            int dataIndex = changedIndex % m_DataCount;
            //Debug.Log("2 = " + m_MoveIndex);
            if (m_MoveIndex < 0)
            {
                poolIndex = GetReverseIndex(m_MoveIndex, m_PoolItemCount);
                dataIndex = GetReverseIndex(m_MoveIndex, m_DataCount);
            }

            SetPositionPoolItem(poolIndex, changedIndex);

            if (m_CallbackChanged != null)
            {
                m_CallbackChanged(m_PoolItems[poolIndex], dataIndex);
            }
        }
    }

    private int GetReverseIndex(int cur, int max)
    {
        return (max - 1) - (Mathf.Abs(cur + 1) % max);
    }

    private float GetItemSize()
    {
        if (m_IsVertical)
        {
            return m_ItemSize.y;
        }

        return m_ItemSize.x;
    }

    private float GetContentPosition()
    {
        if (m_IsVertical)
        {
            return m_Content.anchoredPosition.y;
        }

        return -m_Content.anchoredPosition.x;
    }

    private void SetPositionPoolItem(int poolIndex, int changedIndex)
    {
        if (m_IsVertical)
        {
            m_PoolItems[poolIndex].anchoredPosition = new Vector2(0.0f, -GetItemSize() * changedIndex + m_Content.sizeDelta.y / 2);
        }
        else
        {
            m_PoolItems[poolIndex].anchoredPosition = new Vector2(GetItemSize() * changedIndex + m_Content.sizeDelta.x / 2, 0.0f);
        }
    }

    public GameObject GetPoolItem(int index)
    {
        return m_PoolItems[index].gameObject;
    }
}
