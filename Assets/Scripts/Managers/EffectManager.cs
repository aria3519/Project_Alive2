using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
	private static EffectManager m_instance;
	public static EffectManager Instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = FindObjectOfType<EffectManager>();
			}

			return m_instance;
		}
	}

	private Queue<ParticleSystem> bigExplosionQueue = new Queue<ParticleSystem>();
	private Queue<ParticleSystem> smallExplosionQueue = new Queue<ParticleSystem>();
	private Queue<ParticleSystem> bossRangeAttackQueue = new Queue<ParticleSystem>();
	[SerializeField] private int maxBigExplosionCount = 5;
	[SerializeField] private int maxSmallExplosionCount = 10;
	[SerializeField] private int maxBossRangeAttackEffect = 3;
	public ParticleSystem RocketTrailEffect;
    public ParticleSystem BigExplosionEffect;
    public ParticleSystem SmallExplosionEffect;
	public ParticleSystem bossRangeAttackEffect;
	public ParticleSystem boss1SPAttackEffect;

    private void Awake()
    {
        
    }
    private void Start()
    {
        for(int i = 0; i < maxBigExplosionCount; i++)
        {
			var effect = Instantiate(BigExplosionEffect, Vector3.zero, Quaternion.identity, transform);
			effect.gameObject.SetActive(false);
			bigExplosionQueue.Enqueue(effect);
		}
		for(int i = 0; i < maxSmallExplosionCount; i++)
        {
			var effect = Instantiate(SmallExplosionEffect, Vector3.zero, Quaternion.identity, transform);
			effect.gameObject.SetActive(false);
			smallExplosionQueue.Enqueue(effect);
        }
		for(int i = 0; i < maxBossRangeAttackEffect; i++)
        {
			var effect = Instantiate(bossRangeAttackEffect, Vector3.zero, Quaternion.identity, transform);
			effect.gameObject.SetActive(false);
			bossRangeAttackQueue.Enqueue(effect);
		}
    }

	public void InsertBigExplosionEffect(ParticleSystem effect)
    {
		effect.gameObject.SetActive(false);
		bigExplosionQueue.Enqueue(effect);
    }

	public ParticleSystem GetBigExplosionEffect()
    {
		var effect = bigExplosionQueue.Dequeue();
		effect.gameObject.SetActive(true);
		return effect;
    }

	public void InsertSmallExplosionEffect(ParticleSystem effect)
    {
		effect.gameObject.SetActive(false);
		smallExplosionQueue.Enqueue(effect);
    }

	public ParticleSystem GetSmallExplosionEffect()
    {
		var effect = smallExplosionQueue.Dequeue();
		effect.gameObject.SetActive(true);
		return effect;
    }

	public void InsertBossEffect(ParticleSystem effect)
	{
		effect.gameObject.SetActive(false);
		bossRangeAttackQueue.Enqueue(effect);
	}

	public ParticleSystem GetBossEffect()
	{
		var effect = bossRangeAttackQueue.Dequeue();
		effect.gameObject.SetActive(true);
		return effect;
	}
}
