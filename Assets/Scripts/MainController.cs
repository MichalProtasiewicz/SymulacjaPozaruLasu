using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    public CordinateController[,] mapElements = new CordinateController[30,30];
    public string[,] nextGenMapElements = new string[30, 30];
    public string[,] StartGenMapElements = new string[30, 30];
    public GameObject[] mapElementsTMP;
    public bool coroutineIsEnding = true;
    public int generationCount;
    public Text generationText;

    void Start()
    {
        generationCount = 1;
        generationText.text = "Generation: " + generationCount;
        mapElementsTMP = GameObject.FindGameObjectsWithTag("Cordinate");

        //Utworzenie tablicy 2-wym i jej kopi z tablicy 1-wym 
        int k = 0;
        for (int i = 0; i < 30; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                mapElements[i, j] = mapElementsTMP[k].GetComponent<CordinateController>();
                nextGenMapElements[i, j] = mapElements[i, j].GetComponent<CordinateController>().cordinateState;
                StartGenMapElements[i, j] = mapElements[i, j].GetComponent<CordinateController>().cordinateState;
                k++;
            }
        }
        autoIgnition(); //Pierwszy pozar
    }

    void Update()
    {   
        //Jesli generacja skonczyla sie, wykonujemy kolejna generacje
        if(coroutineIsEnding)
        {
            CheckMapElementState();
            StartCoroutine(GenerateNextMapGeneration());
            generationCount++;
            generationText.text = "Generation: " + generationCount;
            //Autozapłon z prawdopodobienstwem P
            int randomFire = Random.Range(0, 101);
            if (randomFire > 89)
                autoIgnition();
        }
    }

    //Aktualizacja obecnej mapy
    IEnumerator GenerateNextMapGeneration()
    {
        coroutineIsEnding = false;

        for (int i = 0; i < mapElements.GetLength(0); i++)
        {
            for (int j = 0; j < mapElements.GetLength(1); j++)
            {
                mapElements[i, j].cordinateState = nextGenMapElements[i, j];

                if (mapElements[i, j].cordinateState == "tree")
                    mapElements[i, j].sprite.color = Color.green;
                else if (mapElements[i, j].cordinateState == "burn")
                    mapElements[i, j].sprite.color = Color.red;
                else if (mapElements[i, j].cordinateState == "burned")
                    mapElements[i, j].sprite.color = Color.gray;
                else if (mapElements[i, j].cordinateState == "water")
                    mapElements[i, j].sprite.color = Color.blue;
            }
        }
        yield return new WaitForSeconds(1);

        coroutineIsEnding = true;
    }

    //Utworzenie nastepnej generacji
    public void CheckMapElementState()
    {
        for (int i = 0; i < mapElements.GetLength(0); i++)
        {
            for (int j = 0; j < mapElements.GetLength(1); j++)
            {
                //Jesli jest spalone -> sprawdzamy sasiednie drzewa a obecne zostaje spalone w nastepnej generacji
                if (mapElements[i, j].cordinateState == "burn")
                {
                    CheckNeighbour(i, j);
                    nextGenMapElements[i, j] = "burned";
                }
                //Jesli jest spalone dluzej niz 15 generacji, istnieje prawdopodobienstwo, ze drzewo odrosnie
                else if (mapElements[i, j].cordinateState == "burned")
                {
                    mapElements[i, j].burnedCount++;

                    //Odrodzenie drzewa z prawdopodobienstwem P po X generacjach po spaleniu
                    if(mapElements[i, j].burnedCount > 15)
                    {
                        int randomTreePlantCount = Random.Range(0, 101);
                        if (randomTreePlantCount > 50)
                        {
                            mapElements[i, j].burnedCount = 0;
                            nextGenMapElements[i, j] = "tree";
                        }
                    } 
                }
            }
        }
    }

    //Sprawdzenie czy sasiednie kordynaty sa drzewami -> zmiana stanu z prawdopodobienstwem P w nastepnej generacji
    void CheckNeighbour(int CellX, int CellY)
    {
        int minX = Mathf.Max(CellX - 1, mapElements.GetLowerBound(0));
        int maxX = Mathf.Min(CellX + 1, mapElements.GetUpperBound(0));
        int minY = Mathf.Max(CellY - 1, mapElements.GetLowerBound(1));
        int maxY = Mathf.Min(CellY + 1, mapElements.GetUpperBound(1));

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                if (mapElements[x, y].cordinateState == "tree")
                {
                    int randomNumber = Random.Range(0, 101);
                    if(randomNumber > 40)
                        nextGenMapElements[x, y] = "burn";
                }
            }
        }
    }

    //Autozaplon losowego drzewa
    void autoIgnition()
    {
        int randomX;
        int randomY;
        do
        {
            randomX = Random.Range(0, mapElements.GetLength(0) - 1);
            randomY = Random.Range(0, mapElements.GetLength(1) - 1);
        } while (mapElements[randomX, randomY].cordinateState != "tree");

        mapElements[randomX, randomY].cordinateState = "burn";
    }
}