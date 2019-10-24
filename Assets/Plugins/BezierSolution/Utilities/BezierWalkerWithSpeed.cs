using UnityEngine;
using UnityEngine.Events;

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

		private void Awake()
		{
			cachedTransform = transform;
		}

		private void Update()
		{
			float targetSpeed = ( isGoingForward ) ? speed : -speed;

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

				cachedTransform.rotation = Quaternion.Lerp( cachedTransform.rotation, targetRotation, rotationLerpModifier * Time.deltaTime );
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
				if( progress <= startProgress)
				{
					if( !onPathCompletedCalledAt0 )
					{
						onPathCompleted.Invoke();
						onPathCompletedCalledAt0 = true;
					}

					if( travelMode == TravelMode.Once )
						progress = startProgress;
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

    // This must be called only after assigning the Spline
    public void SetStartAndEndPoints(int startPoint, int endPoint = -1)
    {
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
        if (endPoint < startPoint)
        {
          Debug.LogWarning("End point assigned before start point! Start point assigned: " + startPoint + " End point assigned: " + endPoint);
          endPoint = startPoint + 1;
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
      }

      transform.position = spline[startPoint].position;
      transform.rotation = Quaternion.LookRotation(spline.GetTangent(progress));
    } // func end

    public bool TargetReached()
    {
      if (progress >= endProgress)
        return true;

      else return false;
    }
  }
}