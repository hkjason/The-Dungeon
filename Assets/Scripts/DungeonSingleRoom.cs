using UnityEngine;

public class DungeonSingleRoom : MonoBehaviour
{
    public Room aRoom;


    public DungeonSingleRoom DeepCopy()
    {
        DungeonSingleRoom copy = new DungeonSingleRoom();
        copy.aRoom = aRoom.DeepCopy();

        return copy;
    }
}
