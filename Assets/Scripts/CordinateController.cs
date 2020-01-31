using UnityEngine;

public class CordinateController : MonoBehaviour
{
    public SpriteRenderer sprite;
    public string cordinateState;
    public int burnedCount=0;     //Zmienna przechowujaca ilosc generacji po spaleniu drzewa

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        //Wczytanie stanow kordynatow na podstawie kolorow mapy
        if (sprite.color == Color.green)
            cordinateState = "tree";
        if (sprite.color == Color.red)
            cordinateState = "burn";
        if (sprite.color == Color.grey)
            cordinateState = "burned";
        if (sprite.color == Color.blue)
            cordinateState = "water";
    }
}
