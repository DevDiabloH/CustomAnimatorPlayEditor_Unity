using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CustomAnimatorPlayEditor : MonoBehaviour
{
    public float[] endPercents;

    private const string DIRECTION_PARAM_NAME = "Direction";
    private const float DEFAULT_SPEED = 1f;
    private float m_NormalizeStart;
    private float m_NormalizeEnd;
    private float m_CurrNormalizeTime;
    private float m_AnimationLength;
    private float m_EndPercent;
    private bool m_IsPlay = true;

    private Animator m_Animator;

    private void Awake()
    {
        Initialize();
    }

    void FixedUpdate()
    {
        if(null == m_Animator)
        {
            return;
        }

        if(m_AnimationLength == 0 || float.IsInfinity(m_AnimationLength))
        {
            m_AnimationLength = m_Animator.GetCurrentAnimatorStateInfo(0).length;
            return;
        }

        m_CurrNormalizeTime = m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (m_CurrNormalizeTime < 0 || m_CurrNormalizeTime > m_AnimationLength)
        {
            StopAnimation();
        }

        if(m_NormalizeStart < m_NormalizeEnd)
        {
            if(m_CurrNormalizeTime >= m_NormalizeEnd)
            {
                StopAnimation();
            }
        }
        else if(m_NormalizeEnd < m_NormalizeStart)
        {
            if(m_CurrNormalizeTime <= m_NormalizeEnd)
            {
                StopAnimation();
            }
        }
    }

    private void Initialize()
    {
        m_Animator = GetComponent<Animator>();
        m_Animator.SetFloat(DIRECTION_PARAM_NAME, DEFAULT_SPEED);
    }

    private void OnValidate()
    {
        Initialize();
    }

    private void PlayAnimation()
    {
        m_CurrNormalizeTime = Mathf.Clamp(m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 0f, m_AnimationLength);
        m_NormalizeStart = Mathf.Clamp(m_CurrNormalizeTime, 0f, m_AnimationLength);
        m_NormalizeEnd = m_EndPercent * m_AnimationLength;

        float _direction = (m_NormalizeStart < m_NormalizeEnd ? 1f : -1f);
        SetSpeed(_direction);
        m_IsPlay = true;
    }
   
    public void PlayAnimationByNormalizeTime(float _NewNormalizeTime)
    {
        m_EndPercent = _NewNormalizeTime;
        PlayAnimation();
    }

    private void StopAnimation()
    {
        if(true == m_IsPlay)
        {
            m_IsPlay = false;
            m_Animator.SetFloat(DIRECTION_PARAM_NAME, 0);
        }
    }

    private void SetSpeed(float _NewFloat)
    {
        m_Animator.SetFloat(DIRECTION_PARAM_NAME, _NewFloat);
    }

    private void OnGUI()
    {
        float posX = Screen.width * 0.05f;
        float posY = Screen.height * 0.1f;
        float width = 50f;
        float height = 50f;
        float _interval = 100f;

        if (GUI.Button(new Rect(posX, posY, width, height), "0"))
        {
            m_EndPercent = 0f;
            PlayAnimation();
        }

        for(int i=0; i<endPercents.Length; i++)
        {
            string text = string.Empty;

            if(endPercents[i].ToString().Length > 4)
            {
                text = endPercents[i].ToString().Substring(0, 4);
            }
            else
            {
                text = endPercents[i].ToString();
            }

            if (GUI.Button(new Rect(posX + ((i+1) * _interval), posY, width, height), text))
            {
                m_EndPercent = endPercents[i];
                PlayAnimation();
            }
        }

        if(GUI.Button(new Rect(posX + (_interval * (endPercents.Length + 1)), posY, width, height), "1"))
        {
            m_EndPercent = 1f;
            PlayAnimation();
        }
    }
}
