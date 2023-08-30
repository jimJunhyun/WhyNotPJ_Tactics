using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPathMaker : MonoBehaviour
{
	private Camera cam;
	private Plane board;

	private Vector3Int prevPos;
	private Vector3Int curPos;
	private float enter;

	private UnitMover selectedMover;

	[Header("#Setting Values")]
	[SerializeField]
	private LayerMask obstacleLayer;

	[Header("#Others")]
	[SerializeField]
	private List<Vector3Int> pathes = new List<Vector3Int>();
	private bool findPath = false;

	[SerializeField]
	private int moveable = -1;

	private void Awake()
	{
		cam = Camera.main;
		board = new();
		board.SetNormalAndPosition(Vector3.forward, new Vector3(0, 0, 10));
	}

	public void SetActive(bool active)
	{
		if (active == findPath) return;

		if (active == true)
		{
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit2D hit;
			hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, 20f);

			if (hit.collider is null) return;

			if (hit.collider.TryGetComponent(out selectedMover))
			{
				if (!selectedMover.controlable)
				{
					selectedMover = null;
					return;
				}
			}

			if (board.Raycast(ray, out enter))
			{
				findPath = true;
				Vector3 hitPoint = ray.GetPoint(enter);
				curPos = Vector3Int.RoundToInt(hitPoint);
				prevPos = curPos;
				pathes.Add(curPos);
			}
			else
			{
				findPath = false;
			}
		}
		else
		{
			if (selectedMover is not null && moveable == -1)
			{
				pathes.RemoveAt(0);
				selectedMover.SetPath(pathes);
			}

			moveable = -1;
			selectedMover = null;
			findPath = false;
			pathes.Clear();
		}
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0)) // 좌클릭시 이동 시작
		{
			SetActive(true);
		}
		if (Input.GetMouseButtonDown(1)) // 우클릭시 이동 종료
		{
			SetActive(false);
		}

		PathFinding();
	}

	private void PathFinding()
	{
		if (!findPath) return;

		Vector3 hitPoint = cam.ScreenToWorldPoint(Input.mousePosition); // ray.GetPoint(enter);
		hitPoint.z = 10;
		Vector3Int nextPos = Vector3Int.RoundToInt(hitPoint);

		if (nextPos != pathes[0] && Physics2D.CircleCast((Vector3)nextPos, 0.2f, Vector2.zero, 20f, obstacleLayer))
		{
			moveable = pathes.Count;
		}

		if (pathes[pathes.Count - 1].x != nextPos.x || pathes[pathes.Count - 1].y != nextPos.y)
		{
			Vector3Int lastPos = curPos;
			int move = Mathf.Abs(lastPos.x - nextPos.x) + Mathf.Abs(lastPos.y - nextPos.y);
			int x = 0, y = 0, xAdd = nextPos.x < lastPos.x ? -1 : 1, yAdd = nextPos.y < lastPos.y ? -1 : 1;

			for (int i = 1; i <= move; i++) // 한 프레임에 두 칸 이상을 이동했을 때 한 번에 여러칸을 뛰어넘는 것을 방지하는 코드
			{
				// i % 2 연산을 하는 이유는 움직임이 지그재그로 움직이게 하기 위해서이다.
				if (i % 2 == 1 && (lastPos.x + x) != nextPos.x)
				{
					x += xAdd;
				}
				else if (i % 2 == 0 && (lastPos.y + y) != nextPos.y)
				{
					y += yAdd;
				}
				else if ((lastPos.x + x) != nextPos.x)
				{
					x += xAdd;
				}
				else if ((lastPos.y + y) != nextPos.y)
				{
					y += yAdd;
				}

				NextPath(lastPos + new Vector3Int(x, y, 0));
			}
		}

		NextPath(nextPos);
	}

	private void NextPath(Vector3Int nextPos)
	{
		if (pathes.Count > 1 && nextPos.Equals(pathes[pathes.Count - 2]))
		{
			prevPos = pathes[pathes.Count - 2];
			pathes.RemoveAt(pathes.Count - 1);

			if (moveable >= pathes.Count)
			{
				moveable = -1;
			}
		}
		else if (!nextPos.Equals(pathes[pathes.Count - 1]))
		{
			if (pathes.Count >= 64)
			{
				print("Max Path Count!!");
				return;
			}
			prevPos = curPos;
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
