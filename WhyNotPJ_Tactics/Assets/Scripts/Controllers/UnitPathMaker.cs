using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPathMaker : MonoBehaviour
{
	private Camera cam;
	private Plane board;

	private Vector3 prevPos;
	private Vector3 curPos;
	private float enter;

	private UnitBasic selectedMover;

	[Header("#Setting Values")]
	[SerializeField]
	private LayerMask obstacleLayer;

	[Header("#Others")]
	[SerializeField]
	private List<Vector3> pathes = new List<Vector3>();
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
				if (!selectedMover.controlable || selectedMover.isMoving == true)
				{
					selectedMover = null;
					return;
				}
			}

			if (board.Raycast(ray, out enter))
			{
				findPath = true;
				Vector3 hitPoint = ray.GetPoint(enter);
				curPos = new Vector3(Mathf.RoundToInt(hitPoint.x * 2f) / 2f, Mathf.RoundToInt(hitPoint.y * 2f) / 2f, hitPoint.z);
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
		if (Input.GetMouseButtonDown(0)) // ��Ŭ���� �̵� ����
		{
			SetActive(true);
		}
		if (Input.GetMouseButtonDown(1)) // ��Ŭ���� �̵� ����
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
		//Vector3 nextPos = (Vector3)Vector3Int.RoundToInt(hitPoint * 2f) / 2f;
		Vector3 nextPos = new Vector3(Mathf.RoundToInt(hitPoint.x * 2f) / 2f, Mathf.RoundToInt(hitPoint.y * 2f) / 2f, 10);
		print(nextPos);

		if (nextPos != pathes[0] && Physics2D.CircleCast(nextPos, 0.2f, Vector2.zero, 20f, obstacleLayer))
		{
			print("��ֹ� ����");
			moveable = pathes.Count;
		}

		if (pathes[pathes.Count - 1].x != nextPos.x || pathes[pathes.Count - 1].y != nextPos.y)
		{
			Vector3 lastPos = curPos;
			int move = Mathf.RoundToInt(Mathf.Abs(lastPos.x - nextPos.x) + Mathf.Abs(lastPos.y - nextPos.y)) * 2;
			float x = 0, y = 0, xAdd = nextPos.x < lastPos.x ? -0.5f : 0.5f, yAdd = nextPos.y < lastPos.y ? -0.5f : 0.5f;

			for (int i = 1; i <= move; i++)
			{
				// (i % 2) <- for zigzag
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

				NextPath(lastPos + new Vector3(x, y, 0));
			}
		}

		NextPath(nextPos);
	}

	private void NextPath(Vector3 nextPos)
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
