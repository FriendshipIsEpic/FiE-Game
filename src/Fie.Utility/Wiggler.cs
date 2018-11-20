using UnityEngine;

namespace Fie.Utility
{
	public class Wiggler
	{
		public enum WiggleTemplate
		{
			WIGGLE_TYPE_MINIMUM,
			WIGGLE_TYPE_SMALL,
			WIGGLE_TYPE_MIDDLE,
			WIGGLE_TYPE_BIG
		}

		private const float MIN_RANDOM_SHAKE_ANGLE_RANGE = 120f;

		private const float MAX_RANDOM_SHAKE_ANGLE_RANGE = 240f;

		private float nowAngle;

		private float nowTime;

		private float totalTime;

		private float wiggleSegmentTime;

		private int totalWiggleCount = 1;

		private int nowWiggleCount;

		private Vector3 wiggleScale = Vector3.zero;

		private Vector3 wiggleNormal = Vector3.zero;

		private Vector3 nowWigglePoint = Vector3.zero;

		private Vector3 nextWigglePoint = Vector3.zero;

		public bool isEnd => nowWiggleCount >= totalWiggleCount;

		public Wiggler(Vector3 normal, float initTotalTime, int initWiggleCount, Vector3 initWiggleScale)
		{
			totalTime = Mathf.Max(initTotalTime, 0f);
			totalWiggleCount = Mathf.Max(initWiggleCount, 1);
			wiggleScale = initWiggleScale;
			wiggleNormal = normal.normalized;
			InitializeGeneralParams();
		}

		public Wiggler(Vector3 normal, WiggleTemplate template)
		{
			float num = 0f;
			int num2 = 1;
			Vector3 zero = Vector3.zero;
			switch (template)
			{
			default:
				num = 0.15f;
				num2 = 4;
				zero = new Vector3(0.015f, 0.015f);
				break;
			case WiggleTemplate.WIGGLE_TYPE_SMALL:
				num = 0.2f;
				num2 = 5;
				zero = new Vector3(0.03f, 0.03f);
				break;
			case WiggleTemplate.WIGGLE_TYPE_MIDDLE:
				num = 0.3f;
				num2 = 7;
				zero = new Vector3(0.1f, 0.1f);
				break;
			case WiggleTemplate.WIGGLE_TYPE_BIG:
				num = 0.5f;
				num2 = 10;
				zero = new Vector3(0.1f, 0.1f);
				break;
			}
			totalTime = Mathf.Max(num, 0f);
			totalWiggleCount = Mathf.Max(num2, 1);
			wiggleScale = zero;
			wiggleNormal = normal.normalized;
			InitializeGeneralParams();
		}

		public Vector3 UpdateWiggler(float updateTime)
		{
			if (nowTime >= totalTime)
			{
				return Vector3.zero;
			}
			Vector3 result = (!(wiggleSegmentTime > 0f)) ? Vector3.zero : Vector3.Lerp(nowWigglePoint, nextWigglePoint, Mathf.Min(nowTime / wiggleSegmentTime, 1f));
			nowTime += updateTime;
			if (nowTime > wiggleSegmentTime && nowWiggleCount < totalWiggleCount)
			{
				nowWiggleCount++;
				SetNextWigglePoint(nowWiggleCount);
				nowTime = 0f;
			}
			return result;
		}

		private void InitializeGeneralParams()
		{
			nowTime = 0f;
			wiggleSegmentTime = totalTime / (float)totalWiggleCount;
			nowWiggleCount = 0;
			SetNextWigglePoint(nowWiggleCount);
		}

		private void SetNextWigglePoint(int count)
		{
			nowWigglePoint = new Vector3(nextWigglePoint.x, nextWigglePoint.y);
			float num = ((float)totalWiggleCount - (float)count) / (float)totalWiggleCount;
			float angle = nowAngle + Random.Range(120f, 240f);
			Quaternion rotation = Quaternion.AngleAxis(angle, wiggleNormal);
			Vector3 vector = rotation * Vector3.up;
			vector.Normalize();
			nextWigglePoint = new Vector3(wiggleScale.x * (vector.x * num), wiggleScale.y * (vector.y * num));
			nowAngle = angle;
		}
	}
}
