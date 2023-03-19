using Godot;

public class Utitlies
{
    public static Vector3 MulitplyVectorByFloat(Vector3 vector, float multiplyByValue) => 
            new Vector3(vector.X * multiplyByValue, vector.Y * multiplyByValue, vector.Z * multiplyByValue);

    public static void RemoveChildren(Node obj){
        foreach (var item in obj.GetChildren())
			{
  				item.QueueFree();
			}
    }
}