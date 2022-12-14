using Cinemachine.Utility;
using Landfall.TABS.GameState;
using Photon.Bolt;
using UnityEngine;

public class FloatyPhysics : MonoBehaviour
{
	public LayerMask mask;

	public AnimationCurve flightCurve;

	public float heightVariance = 0.5f;

	public float variationSpeed = 0.5f;

	public float flightForce;

	public float legForceMultiplier = 1f;

	private DataHandler data;

	private Rigidbody rightFootRig;

	private Rigidbody leftFootRig;

	private Rigidbody headRig;

	private float distance;

	public float headM = 0.5f;

	private float time;

	private bool dead;

	private bool hasHalvedForce;

	public bool useWings = true;

	public bool useWingsInPlacement = true;

	[Tooltip("Enable if units move erratically on the client side of ProjectMars games. Only enable if you are sure Wings.cs is causing erratic movement.")]
	public bool setUnitMainRigKinematic;

	private GameStateManager m_gameStateManager;

	public float rotationTorque = 10f;

	private void Start()
	{
		data = base.transform.root.GetComponentInChildren<DataHandler>();
		data.takeFallDamage = false;
		if ((bool)data.footRight)
		{
			rightFootRig = data.footRight.GetComponent<Rigidbody>();
		}
		if ((bool)data.footLeft)
		{
			leftFootRig = data.footLeft.GetComponent<Rigidbody>();
		}
		if ((bool)data.head)
		{
			headRig = data.head.GetComponent<Rigidbody>();
		}
		heightVariance *= Random.value;
		time = Random.Range(0f, 1000f);
		m_gameStateManager = ServiceLocator.GetService<GameStateManager>();
		if (setUnitMainRigKinematic && BoltNetwork.IsClient)
		{
			data.mainRig.isKinematic = true;
		}

		distance = Vector3.Distance(data.hip.position,
			new Vector3(data.hip.position.x,
				data.footLeft.GetComponentsInChildren<Collider>()[0].bounds.min.y,
				data.hip.position.z));

		flightForce *= data.unit.unitBlueprint.sizeMultiplier * data.unit.unitBlueprint.sizeMultiplier *
		               (data.unit.unitBlueprint.sizeMultiplier >= 1.4f
			               ? data.unit.unitBlueprint.sizeMultiplier * data.unit.unitBlueprint.sizeMultiplier
			               : 1f);
		if (data.unit.gameObject.name.Contains("Blackbeard"))
		{
			flightForce *= 2f * 2f * 2f;
		}
	}

	private void FixedUpdate()
	{
		if ((!useWingsInPlacement && m_gameStateManager.GameState != GameState.BattleState) || !useWings)
		{
			return;
		}

		if (!data.legLeft.gameObject.activeSelf && !data.legRight.gameObject.activeSelf)
		{
			return;
		}

		if (!hasHalvedForce && ((!data.legLeft.gameObject.activeSelf && data.legRight.gameObject.activeSelf) ||
		    (data.legLeft.gameObject.activeSelf && !data.legRight.gameObject.activeSelf)))
		{
			hasHalvedForce = true;
			flightForce *= 0.3f;
		}
		
		if ((bool)data && data.Dead)
		{
			if (!dead)
			{
				foreach (var rig in data.allRigs.AllRigs)
				{
					rig.velocity = new Vector3(rig.velocity.x, 0f, rig.velocity.z);
				}
				dead = true;
			}
		}

		bool value = data.unit.m_PreferedDistance > data.distanceToTarget;

		RaycastHit hitInfo;
		Physics.Raycast(new Ray(base.transform.position, Vector3.down), out hitInfo, distance, mask);
		if (transform.root.GetComponent<Mount>() && transform.root.GetComponentInChildren<StandingHandler>()) transform.root.GetComponentInChildren<StandingHandler>().enabled = true;
		else if (transform.root.GetComponentInChildren<StandingHandler>()) transform.root.GetComponentInChildren<StandingHandler>().enabled = false;
		if ((bool)hitInfo.transform)
		{
			float num = hitInfo.distance + Mathf.Cos((Time.time + time) * variationSpeed) * heightVariance;
			data.mainRig.AddTorque(rotationTorque * Vector3.Angle(data.mainRig.transform.up, data.groundedMovementDirectionObject.forward) * Vector3.Cross(data.mainRig.transform.up, data.groundedMovementDirectionObject.forward), ForceMode.Acceleration);
			if ((bool)headRig)
			{
				headRig.AddForce(Vector3.up * flightForce * headM * flightCurve.Evaluate(num) * data.ragdollControl, ForceMode.Acceleration);
			}
			data.mainRig.AddForce(Vector3.up * flightForce * flightCurve.Evaluate(num) * data.ragdollControl, ForceMode.Acceleration);
			if ((bool)rightFootRig)
			{
				rightFootRig.AddForce(Vector3.up * flightForce * legForceMultiplier * 0.5f * flightCurve.Evaluate(num) * data.ragdollControl, ForceMode.Acceleration);
			}
			if ((bool)rightFootRig)
			{
				leftFootRig.AddForce(Vector3.up * flightForce * legForceMultiplier * 0.5f * flightCurve.Evaluate(num) * data.ragdollControl, ForceMode.Acceleration);
			}
			data.TouchGround(hitInfo.point, hitInfo.normal);
		}
	}

	public void EnableFlight()
	{
		useWings = true;
	}

	public void DiableFlight()
	{
		useWings = false;
	}
}
