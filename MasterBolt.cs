using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MasterBolt : ProjectileHitEffect
{
	public UnityEvent hitEvent;

	public UnityEvent disableEvent;

	public LayerMask mask;

	private MoveTransform move;

	private ProjectileHit projectileHit;

	private RaycastTrail trail;

	public ParticleSystem part;

	public bool spawnMore = true;

	public bool disableIfNoTarget;

	public bool onlyTargetAlive;

	private List<DataHandler> hitUnits = new List<DataHandler>();

	public float chanceOfSuccess = 1f;

	private void Start()
	{
		projectileHit = GetComponent<ProjectileHit>();
		move = GetComponent<MoveTransform>();
		trail = GetComponent<RaycastTrail>();
	}

	public override bool DoEffect(HitData hit)
	{
		if (!base.gameObject)
		{
			return false;
		}
		if (hitEvent != null)
		{
			hitEvent.Invoke();
		}
		if (!hit.rigidbody)
		{
			part.Play();
			trail.enabled = false;
			move.enabled = false;
			projectileHit.done = true;
			return true;
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, 20f, mask);
		Transform transform = null;
		float num = 999f;
		for (int i = 0; i < array.Length; i++)
		{
			if (!(base.transform.InverseTransformPoint(array[i].transform.position).z > 0.6f) || !(Vector3.Angle(base.transform.forward, array[i].transform.position - base.transform.position) < 90f))
			{
				continue;
			}
			DataHandler componentInParent = array[i].transform.GetComponentInParent<DataHandler>();
			if ((bool)componentInParent)
			{
				if (!componentInParent.Dead && !hitUnits.Contains(componentInParent))
				{
					hitUnits.Add(componentInParent);
				}
				if (componentInParent.Dead)
				{
					continue;
				}
			}
			float num2 = Vector3.Distance(base.transform.position, array[i].transform.position);
			if (num2 < num)
			{
				transform = array[i].transform;
				num = num2;
			}
		}
		if (transform != null)
		{
			if (Random.value < chanceOfSuccess && spawnMore)
			{
				Object.Instantiate(base.gameObject, base.transform.position, base.transform.rotation);
			}
			move.velocity = (transform.position - base.transform.position).normalized * move.selfImpulse.magnitude;
		}
		else if (disableIfNoTarget)
		{
			projectileHit.DisableProjectile();
			disableEvent.Invoke();
		}
		CheckAchievements();
		return true;
	}

	private void CheckAchievements()
	{
		AchievementService service = ServiceLocator.GetService<AchievementService>();
		int num = 0;
		foreach (DataHandler hitUnit in hitUnits)
		{
			if (hitUnit.Dead)
			{
				num++;
			}
		}
		if (num >= 20)
		{
			service.UnlockAchievement("ZEUS_KILL_UNITS");
		}
	}
}
