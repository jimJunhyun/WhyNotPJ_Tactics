using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMoveController : MonoBehaviour
{
	private Camera cam;
	private Plane board;

	private Vector3Int prevPos;
	private Vector3Int curPos;
	private float enter;

	[SerializeField]
	private List<Vector3Int> pathes = new List<Vector3Int>();
	private bool findPath = false;

	private void Awake()
	{
		cam = Camera.main;
		board = new();
		board.SetNormalAndPosition(Vector3.forward, new Vector3(0, 0, 10));
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0)) // 좌클릭시 이동 시작
		{
			Ray r = cam.ScreenPointToRay(Input.mousePosition);
			if (board.Raycast(r, out enter))
			{
				findPath = true;
				Vector3 hitPoint = r.GetPoint(enter);
				curPos = Vector3Int.RoundToInt(hitPoint);
				prevPos = curPos;
				pathes.Add(curPos);
			}
			else
			{
				findPath = false;
			}
		}
		if (Input.GetMouseButtonDown(1)) // 우클릭시 이동 종료
		{
			findPath = false;
			pathes.Clear();
		}

		if (!findPath) return;

		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		if (board.Raycast(ray, out enter))
		{
			Vector3 hitPoint = ray.GetPoint(enter);
			Vector3Int nextPos = Vector3Int.RoundToInt(hitPoint);

			if (pathes[pathes.Count - 1].x != nextPos.x || pathes[pathes.Count - 1].y != nextPos.y)
			{
				Vector3Int prev = pathes[pathes.Count - 1];
				int move = Mathf.Abs(prev.x - nextPos.x) + Mathf.Abs(prev.y - nextPos.y);
				int x = 0, y = 0, xAdd = nextPos.x < prev.x?-1:1, yAdd = nextPos.y < prev.y?-1:1;
				
				for (int i = 1; i <= move; i++) // 한 프레임에 두 칸 이상을 이동했을 때 대각선으로 이동하는 것을 예방하기 위한 코드
				{
					print($"이전: {{{prev.x}, {prev.y}}} 이후: {{{nextPos.x}, {nextPos.y},}} 차이: {{{move - 1}}}");
					// i % 2 연산을 하는 이유는 움직임이 지그재그로 움직이게 하기 위해서이다.
					if (i % 2 == 1 && (prev.x + x) != nextPos.x)
					{
						x += xAdd;
					}
					else if (i % 2 == 0 && (prev.y + y) != nextPos.y)
					{
						y += yAdd;
					}
					else if ((prev.x + x) != nextPos.x)
					{
						x += xAdd;
					}
					else if ((prev.y + y) != nextPos.y)
					{
						y += yAdd;
					}

					NextPath(prev + new Vector3Int(x, y, 0));
				}
			}

			NextPath(nextPos);
		}
	}

	private void NextPath(Vector3Int nextPos)
	{
		if (pathes.Count > 1 && nextPos.Equals(pathes[pathes.Count - 2]))
		{
			pathes.RemoveAt(pathes.Count - 1);
		}
		else if (!nextPos.Equals(pathes[pathes.Count - 1]))
		{
			pathes.Add(nextPos);
		}
		curPos = nextPos;
	}

	private void OnDrawGizmos()
	{
		if (pathes.Count > 1)
		{
			float r = 1f;
			float g = 0;
			float b = 0;
			for (int i = 1; i < pathes.Count; i++)
			{
				Gizmos.color = new Color(r, g, b);
				if (g < 1.0f)
					g += 0.02f;
				else if (r > 0)
					r -= 0.02f;
				else
					b += 0.02f;
				Gizmos.DrawLine(pathes[i - 1], pathes[i]);
			}
		}
	}
}
