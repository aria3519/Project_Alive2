using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
	public Vector2Int gridSize;
	public float cellRadius = 0.5f;
	public FlowField curFlowField;
	//public GridDebug gridDebug;
	private Character player;
	private bool isWaveStarted = false;

    private void InitializeFlowField()
	{
		curFlowField = new FlowField(cellRadius, gridSize);
		curFlowField.CreateGrid();
		//gridDebug.SetFlowField(curFlowField);
	}

    private void OnEnable()
    {
		player = GameManager.instance.GetPlayer();
    }
    private void Update()
	{
		if (GameManager.instance.isWaveOn)
			isWaveStarted = true;
		//player = UnitManager.instance.GetPlayer();

		InitializeFlowField();

		curFlowField.CreateCostField();

		if (isWaveStarted)
		{
			if (player == null)
				return;
			Cell destinationCell = curFlowField.GetCellFromWorldPos(player.transform.position);
			curFlowField.CreateIntegrationField(destinationCell);
		}
		curFlowField.CreateFlowField();

		//gridDebug.DrawFlowField();



		//Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
		//Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
		//Cell destinationCell = curFlowField.GetCellFromWorldPos(player.transform.position);
		//curFlowField.CreateIntegrationField(destinationCell);
	}

	public void SetPlayer(Character getPlayer)
	{
		player = getPlayer;
	}

	//private void Start()
	//   {
	//	InitializeFlowField();

	//	curFlowField.CreateCostField();
	//	curFlowField.CreateFlowField();
	//}

}
