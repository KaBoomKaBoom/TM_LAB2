using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Instantiater : MonoBehaviour
{   
    public GameObject cellTemplate;
    public GameObject cellLeader;
    public GameObject cellWarriot;
    public GameObject cellUndead;

    public int count = 0;

    public float generationInterval = 0.5f;
    public static bool pause = false;

	public int generationCount = 0; // Generation counter
	public Text generationCountText; // Reference to the UI Text element
    public InputField generationCountInputField;

	int[ , ] cellsArray;

    public int gridHeight;
    public int gridWidth;

    public GameObject FertileZone;
    public GameObject DeadZone;
    Vector3 fertilePos;
    Vector3 deadPos;

    public static bool zones = true;


    private Camera cam;

    private float cellSize;

	public Button simulateButton;
	void Update()
    {
		
		// Check if the 's' key is pressed
		if (Input.GetKeyDown(KeyCode.S))
        {
            // Start the program
            StartProgram();
        }

        // Check if the 'g' key is pressed to fill random cells
        if (Input.GetKeyDown(KeyCode.G))
        {
            // Fill random cells in the area
            FillRandomArea();
        }
    }


	// Method to fill a random area with cells
	public void FillRandomArea()
    {
        int startX = UnityEngine.Random.Range(0, gridWidth - 1); // Random start X position
        int startY = UnityEngine.Random.Range(0, gridHeight - 1); // Random start Y position
        int areaWidth = UnityEngine.Random.Range(20, 60); // Random width of the area
        int areaHeight = UnityEngine.Random.Range(20, 60); // Random height of the area

        for (int i = startY; i < startY + areaHeight && i < gridHeight; i++)
        {
            for (int j = startX; j < startX + areaWidth && j < gridWidth; j++)
            {
                cellsArray[i, j] = UnityEngine.Random.Range(0, 2); // Randomly set cell state (0 or 1)
            }
        }
    }
    public void FillRandomCells()
    {
        // Clear existing cells
        ClearCells();

        // Fill random alive cells
        for (int i = 0; i < 1500; i++)
        {
            cellsArray[UnityEngine.Random.Range(0, gridHeight-1), UnityEngine.Random.Range(0, gridWidth-1)] = 1;
        }

        // Render cells after filling random cells
        RenderCells();
    }


    // Method to start the program
   public void StartProgram()
    {
        print(gridWidth + " x " + gridHeight);
        gridWidth = Mathf.RoundToInt(gridHeight * Camera.main.aspect);

        cellsArray = new int[gridHeight, gridWidth];
        cellSize = (Camera.main.orthographicSize * 2) / gridHeight;
        print ("Dimensions: " + gridWidth + "X" + gridHeight);
        print("Cell size: " + cellSize);

        FertileZone = GameObject.Find("FertileZone");
        fertilePos = FertileZone.transform.position;
        print(fertilePos.x + " " + fertilePos.y);

        DeadZone = GameObject.Find("DeadZone");
        deadPos = DeadZone.transform.position;
        print(deadPos.x + " " + deadPos.y);

        // fill random alive cells
        for (int i = 0; i < 1500; i++)
        {
            cellsArray[UnityEngine.Random.Range(20, 70), UnityEngine.Random.Range(30, 120)] = 1; 
        }

        cam = Camera.main;

        InvokeRepeating("NewGenerationUpdate", generationInterval, generationInterval);
    }


    void NewGenerationUpdate() {
        int.TryParse(generationCountInputField.text, out int numGenerations);
        if(numGenerations == 0) numGenerations = int.MaxValue;
		if (numGenerations == generationCount)
        {
            if (pause==true) pause = false; else pause = true;
            print(" Pause " + pause);
			UserInput();
		}
        else
        {
            if (pause==false)
            {
                RenderCells();
                ApplyRules();
				generationCount++; // Increment generation counter
				UpdateGenerationCountText(); // Update UI text with generation count
				//Environment();
			}
            UserInput();
        }
    }

    void UserInput()
    {
        if (Input.GetKey("p"))
        {
            generationCount++;
            if (pause == true) pause = false; else pause = true;
            print(" Pause " + pause);
            
        } 
        else if (Input.GetKey("c"))
        {
            ClearCells();
        }
        
        else if (Input.GetMouseButton(0))
        {
            Vector3 point = new Vector3();
            Vector3 mousePos = Input.mousePosition;
            {
                mousePos.y = cam.pixelHeight - mousePos.y;
                point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));

                int x = 0;
                int y = 0;
                x = (int)((point.x - cellSize / 2) / cellSize);
                y = (int)((point.y - cellSize / 2) / cellSize);

                print(" x= " + x);
                print(" y= " + y);
                if (x > 0 && x < gridWidth && y > 0 && y < gridHeight) cellsArray[y, x] = 1;
                if(zones)UserEnvironment(x, y);
            }
            RenderCells();
        }
        else if (Input.GetMouseButton(1))
        {
            Vector3 point = new Vector3();
            Vector3 mousePos = Input.mousePosition;
            {
                mousePos.y = cam.pixelHeight - mousePos.y;
                point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));

                int x = 0;
                int y = 0;
                x = (int)((point.x - cellSize / 2) / cellSize);
                y = (int)((point.y - cellSize / 2) / cellSize);

                print(" x= " + x);
                print(" y= " + y);
                if (x > 0 && x < gridWidth && y > 0 && y < gridHeight) cellsArray[y, x] = 3;
                if(zones)UserEnvironment(x, y);
            }
            RenderCells();
        }

        else if (Input.GetKey("h"))
        {
            Vector3 point = new Vector3();
            Vector3 mousePos = Input.mousePosition;
            {
                mousePos.y = cam.pixelHeight - mousePos.y;
                point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));

                int x = 0;
                int y = 0;
                x = (int)((point.x - cellSize / 2) / cellSize);
                y = (int)((point.y - cellSize / 2) / cellSize);

                print(" x= " + x);
                print(" y= " + y);

                for(int i = y - 10; i <= y + 10; i++)
                {
                    for (int j = x - 10; j <= x + 10; j++)
                    {
                        if (j > 0 && j < gridWidth && i > 0 && i < gridHeight) cellsArray[i, j] = 1;
                    }
                }

                if(zones)UserEnvironment(x, y);
            }
            RenderCells();
        }
        else if (Input.GetKey("w"))
        {
            Vector3 point = new Vector3();
            Vector3 mousePos = Input.mousePosition;
            {
                mousePos.y = cam.pixelHeight - mousePos.y;
                point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));

                int x = 0;
                int y = 0;
                x = (int)((point.x - cellSize / 2) / cellSize);
                y = (int)((point.y - cellSize / 2) / cellSize);

                print(" x= " + x);
                print(" y= " + y);
                if (x > 0 && x < gridWidth && y > 0 && y < gridHeight) cellsArray[y, x] = 2;
                print("God cell");
                if (zones) UserEnvironment(x, y);
            }
            RenderCells();
        }
    }

    void ClearCells(){
        // Reset all cells to 0 (dead)
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                cellsArray[i, j] = 0;
            }
        }
    RenderCells(); // Update the visual representation of cells
    }


    public void RenderCells(){
        foreach (GameObject cell in GameObject.FindGameObjectsWithTag("Cell")){
            Destroy(cell);
        }

        for (int i = 0; i < gridHeight; i++){
            for (int j = 0; j < gridWidth; j++){
                if (cellsArray[i, j] == 0) continue;
                if (cellsArray[i, j] == 1)
                {
                    Vector3 cellPosition = new Vector3(
                        j * cellSize + cellSize / 2,
                        (cellSize * gridHeight) - (i * cellSize + cellSize / 2),
                        0
                    );
                    Instantiate(cellTemplate, cellPosition, Quaternion.identity);
                }
                if (cellsArray[i, j] == 2)
                {
                    Vector3 cellPosition = new Vector3(
                        j * cellSize + cellSize / 2,
                        (cellSize * gridHeight) - (i * cellSize + cellSize / 2),
                        0
                    );
                    Instantiate(cellLeader, cellPosition, Quaternion.identity);
                }
                if (cellsArray[i, j] == 3)
                {
                    Vector3 cellPosition = new Vector3(
                        j * cellSize + cellSize / 2,
                        (cellSize * gridHeight) - (i * cellSize + cellSize / 2),
                        0
                    );
                    Instantiate(cellWarriot, cellPosition, Quaternion.identity);
                }
				if (cellsArray[i, j] == 4)
				{
					Vector3 cellPosition = new Vector3(
						j * cellSize + cellSize / 2,
						(cellSize * gridHeight) - (i * cellSize + cellSize / 2),
						0
					);
					Instantiate(cellUndead, cellPosition, Quaternion.identity);
				}
			}
        }

    }

    public void ApplyRules()
    {


        FertileZone = GameObject.Find("FertileZone");
        fertilePos = FertileZone.transform.position;
        DeadZone = GameObject.Find("DeadZone");
        deadPos = DeadZone.transform.position;


		print("Fert " + fertilePos);
        print("Dead "+deadPos);


        int[,] nextGenGrid = new int[gridHeight, gridWidth];
        if (generationCount % 3 == 0)
        {
            print("undead");
            for (int i = 0; i < 50; i++)
            {
                if (UnityEngine.Random.Range(0f, 1f) > 0.50f)
                {
                    nextGenGrid[UnityEngine.Random.Range(1, gridHeight - 1), UnityEngine.Random.Range(1, gridWidth - 1)] = 4;
                }
            }
		}
        for (int i = 1; i < gridHeight-2; i++)
        {
            for (int j = 1; j < gridWidth-2; j++)
            {
                int livingNeighbours = CountLivingNeighbours(i, j);


                if (ControlGod(i, j))
                {
					nextGenGrid[i - 1, j] = 1;
                    nextGenGrid[i, j - 1] = 1;
                    nextGenGrid[i, j + 1] = 1;
                    nextGenGrid[i + 1, j] = 1;
                    nextGenGrid[i, j] = 2;
                }
                else if (ControlDiablo(i, j))
                {

					int living = CountLivingNeighbours(i, j);
						if (living <= 3 && living!=0 )
						{
							cellsArray[i - 1, j] = 0;
							cellsArray[i, j - 1] = 0;
							cellsArray[i, j + 1] = 0;
							cellsArray[i + 1, j] = 0;
							nextGenGrid[i + 1, j] = 3;
						    nextGenGrid[i - 1, j] = 3;
							nextGenGrid[i, j - 1] = 3;
							nextGenGrid[i, j + 1] = 3;
					}
				}
				else if (cellsArray[i, j] == 4)
				{
					// Kill neighboring cells by setting their states to 0 (dead)
					if (Control(i - 1, j)) nextGenGrid[i - 1, j] = 0;
					if (Control(i + 1, j)) nextGenGrid[i + 1, j] = 0;
					if (Control(i, j - 1)) nextGenGrid[i, j - 1] = 0;
					if (Control(i, j + 1)) nextGenGrid[i, j + 1] = 0;
					if (Control(i - 1, j - 1)) nextGenGrid[i - 1, j - 1] = 0;
					if (Control(i - 1, j + 1)) nextGenGrid[i - 1, j + 1] = 0;
					if (Control(i + 1, j - 1)) nextGenGrid[i + 1, j - 1] = 0;
					if (Control(i + 1, j + 1)) nextGenGrid[i + 1, j + 1] = 0;

					// Mark the cellIntr itself for the next generation
					nextGenGrid[i, j] = 4;
				}
				else if (livingNeighbours == 3)
                { // reproduction, exactly 3 neighgbours
					nextGenGrid[i, j] = 1;
                }
                else if (livingNeighbours == 2 && cellsArray[i, j] == 1)
                { // exactly 2 neigh, the live cell survives
					nextGenGrid[i, j] = 1;
                }
                else if (livingNeighbours > 3 || livingNeighbours<2)
                {
					//die from overpopulation
					nextGenGrid[i, j] = 0;
				}

                if (zones)
                {
                    bool fertile = PosFertile(j * cellSize + cellSize / 2, (cellSize * gridHeight) - (i * cellSize + cellSize / 2));
                    bool dead = PosDead(j * cellSize + cellSize / 2, (cellSize * gridHeight) - (i * cellSize + cellSize / 2));
                    if (dead )
                    {
						//print(i + " " + j + " dead");
						if (cellsArray[i, j] == 4) nextGenGrid[i, j] = 4;
                        else
                        {
                            if (UnityEngine.Random.Range(0f, 1f) > 0.90f)
                            {
                                nextGenGrid[i + 1, j] = 3;
                                nextGenGrid[i - 1, j] = 3;
                                nextGenGrid[i , j + 1] = 3;
                                nextGenGrid[i , j - 1] = 3;


                            }
                        }		
					}
                    else
                        if (fertile)
                    {
						if (cellsArray[i, j] == 4) nextGenGrid[i, j] = 4;
						if (cellsArray[i, j] == 3) nextGenGrid[i, j] = 1;
                        if (livingNeighbours == 2 || livingNeighbours == 3)
                        { // reproduction,  2 to 3 neighgbours
                            nextGenGrid[i, j] = 1;
                        }
                        else if ((livingNeighbours == 1 || livingNeighbours == 2) && cellsArray[i, j] == 1)
                        { // 1 or 2 neigh, the live cell survives
                            nextGenGrid[i, j] = 1;
                        }
                    }
                }
            }
        }
        cellsArray = nextGenGrid; // GOING TO THE NEXT GEN!
    }

    public void UserEnvironment(int x, int y)
    {//fertile zone
        if(x > 5 && x<20 && y > 5 && y < 20)
        {
            for(int i = 5; i <= 20; i++)
                for(int j = 5; j <= 20; j++)
                {
                    cellsArray[j, i] = 1;
                }
        }

        //dead zone


        
    }

    

    bool Control(int x,int y)
    {
        if (x > 0 && x < gridWidth && y > 0 && y < gridHeight) return true; else return false;
    }

    bool PosFertile(float x,float y)
        {

        if (x > fertilePos.x - 3.1 && x < fertilePos.x + 3.1 && y > fertilePos.y - 1 && y < fertilePos.y + 1) {
            //print("fertile"); 
            //print(fertilePos.x + " " + fertilePos.y);
            return true; } else return false;
        }

    bool PosDead(float x,float y)
        {
        if (x > deadPos.x - 3.1 && x < deadPos.x + 3.1 && y > deadPos.y - 1 && y < deadPos.y + 1) { 
            //print(x + " " + y + " "  + deadPos.x  +  " dead"); 
            return true; } else return false;
    }

    bool ControlGod(int x , int y)
    {
        if (cellsArray[x, y] == 2) return true;
        return false;
    }

    bool ControlDiablo(int x,int y)
    {
        if (cellsArray[x, y] == 3) return true;
        return false;
    }

    int CountLivingNeighbours(int i, int j){
        int result = 0;
        for (int iNeigh = i-1; iNeigh <= i+1; iNeigh++){ // i-1, i, i+1
            for (int jNeigh = j-1; jNeigh <= j+1; jNeigh++){ // j-1, j, j+1
                if (cellsArray[iNeigh, jNeigh] != 0  )
                   result += 1;

            }
        }

        return result;
    }
	void UpdateGenerationCountText()
	{
		if (generationCountText != null)
		{
			generationCountText.text = "Generation: " + generationCount.ToString();
		}
	}

}

