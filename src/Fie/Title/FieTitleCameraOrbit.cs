using UnityEngine;

namespace Fie.Title
{
	public class FieTitleCameraOrbit : MonoBehaviour
	{
		public GameObject target;

		public Transform targetFocus;

		public float distance = 1f;

		[Range(0.1f, 4f)]
		public float ZoomWheelSpeed = 4f;

		public float minDistance = 0.5f;

		public float maxDistance = 10f;

		public float xSpeed = 250f;

		public float ySpeed = 120f;

		public float xObjSpeed = 250f;

		public float yObjSpeed = 120f;

		public float yMinLimit = -20f;

		public float yMaxLimit = 80f;

		private float x;

		private float y;

		private float normal_angle;

		private float cur_distance;

		private float cur_xSpeed;

		private float cur_ySpeed;

		private float req_xSpeed;

		private float req_ySpeed;

		private float cur_ObjxSpeed;

		private float cur_ObjySpeed;

		private float req_ObjxSpeed;

		private float req_ObjySpeed;

		private bool DraggingObject;

		private bool lastLMBState;

		private Collider[] surfaceColliders;

		private float bounds_MaxSize = 20f;

		[HideInInspector]
		public bool disableSteering;

		private void Start()
		{
			Vector3 eulerAngles = base.transform.eulerAngles;
			x = eulerAngles.y;
			y = eulerAngles.x;
			Reset();
		}

		public void DisableSteering(bool state)
		{
			disableSteering = state;
		}

		public void Reset()
		{
			lastLMBState = Input.GetMouseButton(0);
			disableSteering = false;
			cur_distance = distance;
			cur_xSpeed = 0f;
			cur_ySpeed = 0f;
			req_xSpeed = 0f;
			req_ySpeed = 0f;
			surfaceColliders = null;
			cur_ObjxSpeed = 0f;
			cur_ObjySpeed = 0f;
			req_ObjxSpeed = 0f;
			req_ObjySpeed = 0f;
			if ((bool)target)
			{
				Renderer[] componentsInChildren = target.GetComponentsInChildren<Renderer>();
				Bounds bounds = default(Bounds);
				bool flag = false;
				Renderer[] array = componentsInChildren;
				foreach (Renderer renderer in array)
				{
					if (!flag)
					{
						flag = true;
						bounds = renderer.bounds;
					}
					else
					{
						bounds.Encapsulate(renderer.bounds);
					}
				}
				Vector3 size = bounds.size;
				float num = (!(size.x > size.y)) ? size.y : size.x;
				num = (bounds_MaxSize = ((!(size.z > num)) ? num : size.z));
				cur_distance += bounds_MaxSize * 1.2f;
				surfaceColliders = target.GetComponentsInChildren<Collider>();
			}
		}

		private void LateUpdate()
		{
			if ((bool)target && (bool)targetFocus)
			{
				if (!lastLMBState && Input.GetMouseButton(0))
				{
					DraggingObject = false;
					if (surfaceColliders != null)
					{
						RaycastHit hitInfo = default(RaycastHit);
						Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
						Collider[] array = surfaceColliders;
						foreach (Collider collider in array)
						{
							if (collider.Raycast(ray, out hitInfo, float.PositiveInfinity))
							{
								DraggingObject = true;
								break;
							}
						}
					}
				}
				else if (lastLMBState && !Input.GetMouseButton(0))
				{
					DraggingObject = false;
				}
				lastLMBState = Input.GetMouseButton(0);
				if (DraggingObject)
				{
					if (Input.GetMouseButton(0) && !disableSteering)
					{
						req_ObjxSpeed += (Input.GetAxis("Mouse X") * xObjSpeed * 0.02f - req_ObjxSpeed) * Time.deltaTime * 10f;
						req_ObjySpeed += (Input.GetAxis("Mouse Y") * yObjSpeed * 0.02f - req_ObjySpeed) * Time.deltaTime * 10f;
					}
					else
					{
						req_ObjxSpeed += (0f - req_ObjxSpeed) * Time.deltaTime * 4f;
						req_ObjySpeed += (0f - req_ObjySpeed) * Time.deltaTime * 4f;
					}
					req_xSpeed += (0f - req_xSpeed) * Time.deltaTime * 4f;
					req_ySpeed += (0f - req_ySpeed) * Time.deltaTime * 4f;
				}
				else
				{
					if (Input.GetMouseButton(0) && !disableSteering)
					{
						req_xSpeed += (Input.GetAxis("Mouse X") * xSpeed * 0.02f - req_xSpeed) * Time.deltaTime * 10f;
						req_ySpeed += (Input.GetAxis("Mouse Y") * ySpeed * 0.02f - req_ySpeed) * Time.deltaTime * 10f;
					}
					else
					{
						req_xSpeed += (0f - req_xSpeed) * Time.deltaTime * 4f;
						req_ySpeed += (0f - req_ySpeed) * Time.deltaTime * 4f;
					}
					req_ObjxSpeed += (0f - req_ObjxSpeed) * Time.deltaTime * 4f;
					req_ObjySpeed += (0f - req_ObjySpeed) * Time.deltaTime * 4f;
				}
				distance -= Input.GetAxis("Mouse ScrollWheel") * ZoomWheelSpeed;
				distance = Mathf.Clamp(distance, minDistance, maxDistance);
				cur_ObjxSpeed += (req_ObjxSpeed - cur_ObjxSpeed) * Time.deltaTime * 20f;
				cur_ObjySpeed += (req_ObjySpeed - cur_ObjySpeed) * Time.deltaTime * 20f;
				target.transform.RotateAround(targetFocus.position, Vector3.Cross(targetFocus.position - base.transform.position, base.transform.right), 0f - cur_ObjxSpeed);
				target.transform.RotateAround(targetFocus.position, Vector3.Cross(targetFocus.position - base.transform.position, base.transform.up), 0f - cur_ObjySpeed);
				cur_xSpeed += (req_xSpeed - cur_xSpeed) * Time.deltaTime * 20f;
				cur_ySpeed += (req_ySpeed - cur_ySpeed) * Time.deltaTime * 20f;
				x += cur_xSpeed;
				y -= cur_ySpeed;
				y = ClampAngle(y, yMinLimit + normal_angle, yMaxLimit + normal_angle);
				if (surfaceColliders != null)
				{
					RaycastHit hitInfo2 = default(RaycastHit);
					Vector3 vector = Vector3.Normalize(targetFocus.position - base.transform.position);
					float num = 0.01f;
					bool flag = false;
					Collider[] array2 = surfaceColliders;
					foreach (Collider collider2 in array2)
					{
						if (collider2.Raycast(new Ray(base.transform.position - vector * bounds_MaxSize, vector), out hitInfo2, float.PositiveInfinity))
						{
							num = Mathf.Max(Vector3.Distance(hitInfo2.point, targetFocus.position) + distance, num);
							flag = true;
						}
					}
					if (flag)
					{
						cur_distance += (num - cur_distance) * Time.deltaTime * 4f;
					}
				}
				Quaternion rotation = Quaternion.Euler(y, x, 0f);
				base.transform.rotation = rotation;
			}
		}

		private static float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360f)
			{
				angle += 360f;
			}
			if (angle > 360f)
			{
				angle -= 360f;
			}
			return Mathf.Clamp(angle, min, max);
		}

		public void set_normal_angle(float a)
		{
			normal_angle = a;
		}
	}
}
