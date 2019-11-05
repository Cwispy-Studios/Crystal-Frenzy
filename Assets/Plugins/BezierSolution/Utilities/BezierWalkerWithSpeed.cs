using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

namespace BezierSolution
{
	public class BezierWalkerWithSpeed : MonoBehaviour, IBezierWalker
	{
		public enum TravelMode { Once, Loop, PingPong };

		private Transform cachedTransform;

		public BezierSpline spline;
		public TravelMode travelMode;

		public float speed = 5f;
		private float progress = 0f;
    private float startProgress = 0f;
    private float endProgress = 1f;

		public BezierSpline Spline { get { return spline; } }

		public float NormalizedT
		{
			get { return progress; }
			set { progress = value; }
		}

		//public float movementLerpModifier = 10f;
		public float rotationLerpModifier = 10f;

		public bool lookForward = true;

		private bool isGoingForward = true;
		public bool MovingForward { get { return ( speed > 0f ) == isGoingForward; } }

		public UnityEvent onPathCompleted = new UnityEvent();
		private bool onPathCompletedCalledAt1 = false;
		private bool onPathCompletedCalledAt0 = false;

    private const float PERC_SLOWDOWN_PER_UNIT = 0.1f;
    private float unitRadius = 5f;

    private const float COLLISION_MARGIN = 0.95f;
    private const float COLLISION_CHECK_INTERVAL = 0.1f;
    private float collisionCountdown = 0;

    private float slowDownPerc = 0;

    private FMODUnity.StudioEventEmitter musicEmitter;

    private FMODUnity.StudioEventEmitter moveEmitter;

    [FMODUnity.EventRef]
    public string footstepSound = "";

    private void Awake()
		{
			cachedTransform = transform;

      unitRadius = GetComponent<SphereCollider>().radius * COLLISION_MARGIN;
      moveEmitter = GetComponent<FMODUnity.StudioEventEmitter>();

      musicEmitter = Camera.main.GetComponent<FMODUnity.StudioEventEmitter>();
    }

		private void Update()
		{
      // Player crystal seeker, update progress
      if (isGoingForward)
      {
        musicEmitter.SetParameter("Player Crystal Seeker Progress", (progress - startProgress) / (endProgress - startProgress));
      }

      // Enemy crystal seeker
      else
      {
        musicEmitter.SetParameter("Enemy Crystal Seeker Progress", (endProgress - progress) / (endProgress - startProgress));
      }

			float targetSpeed = ( isGoingForward ) ? speed : -speed;

      if (collisionCountdown < COLLISION_CHECK_INTERVAL)
      {
        collisionCountdown += Time.deltaTime;
      }

      else
      {
        collisionCountdown = 0;
        CheckCollision();
      }

      targetSpeed *= (1f - slowDownPerc);

			Vector3 targetPos = spline.MoveAlongSpline( ref progress, targetSpeed * Time.deltaTime );

			cachedTransform.position = targetPos;
			//cachedTransform.position = Vector3.Lerp( cachedTransform.position, targetPos, movementLerpModifier * Time.deltaTime );

			bool movingForward = MovingForward;

			if( lookForward )
			{
				Quaternion targetRotation;
				if( movingForward )
					targetRotation = Quaternion.LookRotation( spline.GetTangent( progress ) );
				else
					targetRotation = Quaternion.LookRotation( -spline.GetTangent( progress ) );

				cachedTransform.rotation = Quaternion.Lerp( cachedTransform.rotation, targetRotation, rotationLerpModifier * Time.deltaTime);
			}

			if( movingForward )
			{
				if( progress >= endProgress)
				{
					if( !onPathCompletedCalledAt1 )
					{
						onPathCompleted.Invoke();
						onPathCompletedCalledAt1 = true;
					}

					if( travelMode == TravelMode.Once )
						progress = endProgress;
					else if( travelMode == TravelMode.Loop )
						progress -= 1f;
					else
					{
						progress = 2f - progress;
						isGoingForward = !isGoingForward;
					}
				}
				else
				{
					onPathCompletedCalledAt1 = false;
				}
			}
			else
			{
				if( progress <= endProgress)
				{
					if( !onPathCompletedCalledAt0 )
					{
						onPathCompleted.Invoke();
						onPathCompletedCalledAt0 = true;
					}

					if( travelMode == TravelMode.Once )
						progress = endProgress;
					else if( travelMode == TravelMode.Loop )
						progress += endProgress;
					else
					{
						progress = -progress;
						isGoingForward = !isGoingForward;
					}
				}
				else
				{
					onPathCompletedCalledAt0 = false;
				}
			}
		}

    private void CheckCollision()
    {
      slowDownPerc = 0;

      // Check for objects colliding with the crystal seeker
      Collider[] colliders = Physics.OverlapSphere(transform.position, unitRadius, 1 << 0);

      float angleFront = 90f;

      // Push them away, while calculating how many of them are are in front of the crystal seeker
      for (int i = 0; i < colliders.Length; ++i)
      {
        NavMeshAgent agent = colliders[i].GetComponent<NavMeshAgent>();
        NavMeshObstacle obstacle = colliders[i].GetComponent<NavMeshObstacle>();

        // Check if the object has both a navmesh agent and obstacle, indicating it is a unit
        if (agent != null && obstacle != null)
        {
          // Find the radius of the colliding unit
          float collidingUnitRadius = colliders[i].GetComponent<CapsuleCollider>().radius;

          // Find the direction it is colliding in
          Vector3 direction = colliders[i].transform.position - transform.position;
          float distance = direction.magnitude;

          direction /= distance;

          float angleBetween = Vector3.Angle(transform.forward, direction);

          if (angleBetween <= angleFront)
          {
            slowDownPerc += Mathf.Lerp(0, 0.25f, angleBetween / angleFront);
          }

          bool agentWasEnabled = agent.enabled;
          bool obstacleWasEnabled = obstacle.enabled;

          // Move the unit so that it is just not colliding with the crystal seeker
          obstacle.enabled = false;
          agent.enabled = true;

          Vector3 cacheDestination = agent.destination;
          Vector3 cacheVelocity = agent.velocity;

          agent.Warp(transform.position + (direction * (unitRadius + collidingUnitRadius)));

          if (agentWasEnabled)
          {
            obstacle.enabled = obstacleWasEnabled;
            agent.enabled = agentWasEnabled;
          }

          else if (obstacleWasEnabled)
          {
            agent.enabled = agentWasEnabled;
            obstacle.enabled = obstacleWasEnabled; 
          }

          // Only resume the destination if agent was already enabled
          if (agent.enabled)
          {
            agent.destination = cacheDestination;
            agent.velocity = cacheVelocity;
          }
        }
      }

      Mathf.Clamp(slowDownPerc, 0, 0.9f);
    }

    // This must be called only after assigning the Spline
    public void SetStartAndEndPoints(int startPoint, int endPoint, bool movingForward)
    {
      isGoingForward = movingForward;

      // Ensures we don't set an invalid start point
      if (startPoint > 0 && startPoint < spline.Count - 1)
      {
        startProgress = startPoint / (spline.Count - 1f);
        progress = startProgress;
      }

      else
      {
        startPoint = 0;
        progress = startProgress = 0f;
        Debug.LogWarning("Invalid start point assigned! Point assigned: " + startPoint + " Spline array till: " + (spline.Count - 1));
      }

      if (endPoint == -1)
      {
        endProgress = 1f;
      }

      else
      {
        // Ensures we don't set an invalid end point
        if (endPoint > 0 && endPoint < spline.Count)
        {
          endProgress = endPoint / (spline.Count - 1f);
        }

        else
        {
          endProgress = 1f;
          Debug.LogWarning("Invalid end point assigned! Point assigned: " + endPoint + " Spline array till: " + (spline.Count - 1));
        }
      }

      transform.position = spline[startPoint].position;
      transform.rotation = Quaternion.LookRotation(spline.GetTangent(progress));
    } // func end

    public bool TargetReached()
    {
      if (isGoingForward && progress >= endProgress)
      {
        musicEmitter.SetParameter("Player Crystal Seeker Progress", 1);
        moveEmitter.SetParameter("Moving", 1);
        return true;
      }
      else if (!isGoingForward && progress <= endProgress)
      {
        musicEmitter.SetParameter("Enemy Crystal Seeker Progress", 1);
        moveEmitter.Stop();
        return true;
      }

      else return false;
    }

    private void PlayFootsteps()
    {
      FMODUnity.RuntimeManager.PlayOneShot(footstepSound, transform.position);
    }

    private void OnDestroy()
    {
      GetComponent<Animator>().enabled = false;

      musicEmitter.SetParameter("Player Crystal Seeker Progress", 0);
      musicEmitter.SetParameter("Enemy Crystal Seeker Progress", 0);
      musicEmitter.SetParameter("Enemy Crystal Seeker Health", 1);
    }
  }
}