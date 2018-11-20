using Fie.Object;
using Fie.Utility;
using GameDataEditor;
using TMPro;
using UnityEngine;

namespace Fie.UI
{
	[FiePrefabInfo("Prefabs/UI/GainedScore/FieGainedScoreUI")]
	public class FieGameUIGainedScore : FieEmittableObjectBase
	{
		[SerializeField]
		private float MOVING_SPEED_PAR_SEC = 0.25f;

		[SerializeField]
		private TextMeshPro _textMesh;

		private const float duration = 5f;

		private Tweener<TweenTypesLinear> _alphaTweener = new Tweener<TweenTypesLinear>();

		public override void awakeEmitObject()
		{
			destoryEmitObject(5f);
			_alphaTweener.InitTweener(5f, 1f, 0f);
		}

		public void SetScore(int score, bool isDefeater)
		{
			string empty = string.Empty;
			empty = ((!isDefeater) ? FieLocalizeUtility.GetConstantText(GDEItemKeys.ConstantTextList_GAINED_SCORE_ASSIST) : FieLocalizeUtility.GetConstantText(GDEItemKeys.ConstantTextList_GAINED_SCORE_DEFEAT));
			empty = empty.Replace("___Value1___", score.ToString());
			_textMesh.text = empty;
		}

		private void Update()
		{
			Transform transform = base.transform;
			Vector3 position = base.transform.position;
			float x = position.x;
			Vector3 position2 = base.transform.position;
			float y = position2.y + MOVING_SPEED_PAR_SEC * Time.deltaTime;
			Vector3 position3 = base.transform.position;
			transform.position = new Vector3(x, y, position3.z);
			if (!_alphaTweener.IsEnd())
			{
				float num = _alphaTweener.UpdateParameterFloat(Time.deltaTime);
				_textMesh.color = new Color(1f, 1f, 1f, 1f * num);
			}
			base.transform.rotation = Quaternion.identity;
		}
	}
}
