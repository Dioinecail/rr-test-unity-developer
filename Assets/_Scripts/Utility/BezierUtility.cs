using UnityEngine;

namespace Project.Utility.Math
{
	public static class BezierUtility
	{
		public static void DrawBezier(Transform[] anchors, int quality)
		{
			Gizmos.color = Color.green;

			var positions = new Vector3[anchors.Length];

			for (int i = 0; i < positions.Length; i++)
			{
				positions[i] = anchors[i].position;
			}

			var pos = Bezier(positions, 0);

			for (int i = 0; i < positions.Length; i++)
			{
				Gizmos.DrawWireSphere(positions[i], 0.1f);

				if (i < positions.Length - 1)
				{
					Gizmos.DrawLine(positions[i], positions[i + 1]);
				}
			}

			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(pos, 0.1f);

			for (int i = 0; i < quality; i++)
			{
				var pos1 = Bezier(positions, i / (float)quality);
				var pos2 = Bezier(positions, (i + 1) / (float)quality);

				Gizmos.DrawLine(pos1, pos2);
			}
		}

		public static Vector3 Bezier(Vector3[] positions, float lerp)
		{
			Vector3 p01 = Vector3.Lerp(positions[0], positions[1], lerp);
			Vector3 p12 = Vector3.Lerp(positions[1], positions[2], lerp);
			Vector3 p23 = Vector3.Lerp(positions[2], positions[3], lerp);

			Vector3 l0112 = Vector3.Lerp(p01, p12, lerp);
			Vector3 l1223 = Vector3.Lerp(p12, p23, lerp);

			Vector3 final = Vector3.Lerp(l0112, l1223, lerp);

			return final;
		}

		public static Vector3 Bezier(Transform[] anchors, float lerp)
		{
			var positions = new Vector3[anchors.Length];

			for (int i = 0; i < positions.Length; i++)
			{
				positions[i] = anchors[i].position;
			}

			Vector3 p01 = Vector3.Lerp(positions[0], positions[1], lerp);
			Vector3 p12 = Vector3.Lerp(positions[1], positions[2], lerp);
			Vector3 p23 = Vector3.Lerp(positions[2], positions[3], lerp);

			Vector3 l0112 = Vector3.Lerp(p01, p12, lerp);
			Vector3 l1223 = Vector3.Lerp(p12, p23, lerp);

			Vector3 final = Vector3.Lerp(l0112, l1223, lerp);

			return final;
		}
	}
}