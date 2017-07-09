using UnityEngine;
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dDestroyer))]
	public class D2dDestroyer_Editor : D2dEditor<D2dDestroyer>
	{
		protected override void OnInspector()
		{
			DrawDefault("Life");
			
			Separator();

			DrawDefault("Fade");

			if (Any(t => t.Fade == true))
			{
				BeginIndent();
				{
					BeginError(Any(t => t.FadeDuration <= 0.0f));
					{
						DrawDefault("FadeDuration");
					}
					EndError();
				}
				EndIndent();
			}

			Separator();

			DrawDefault("Shrink");

			if (Any(t => t.Shrink == true))
			{
				BeginIndent();
				{
					BeginError(Any(t => t.ShrinkDuration <= 0.0f));
					{
						DrawDefault("ShrinkDuration");
					}
					EndError();
				}
				EndIndent();
			}
		}
	}
}
