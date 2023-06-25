using Godot;
using Vector3 = Godot.Vector3;

public class Utilities
{

    /// <summary>
    /// A method that multiplies each component of a vector by a given float value and returns a new vector.
    /// </summary>
    /// <param name="vector">The vector to multiply.</param>
    /// <param name="multiplyByValue">The float value to multiply by.</param>
    /// <returns>A new vector with the multiplied components.</returns>
    public static Vector3 MulitplyVectorByFloat(Vector3 vector, float multiplyByValue) =>
                new Vector3(vector.X * multiplyByValue, vector.Y * multiplyByValue, vector.Z * multiplyByValue);

    /// <summary>
    /// A method that divides each component of a vector by a given float value and returns a new vector.
    /// </summary>
    /// <param name="vector">The vector to divide.</param>
    /// <param name="multiplyByValue">The float value to divide by.</param>
    /// <returns>A new vector with the divided components.</returns>
    public static Vector3 DivideVectorByFloat(Vector3 vector, float multiplyByValue) =>
                new Vector3(vector.X / multiplyByValue, vector.Y / multiplyByValue, vector.Z / multiplyByValue);

    /// <summary>
    /// A method that removes all the children of a given node and frees them from memory.
    /// </summary>
    /// <param name="obj">The node to remove the children from.</param>
    public static void RemoveChildren(Node obj)
    {
        // For each child of the node
        foreach (var item in obj.GetChildren())
        {
            // Queue the child for deletion
            item.QueueFree();
        }
    }

    /// <summary>
    /// A method that linearly interpolates between two vectors by a given weight and returns a new vector.
    /// </summary>
    /// <param name="firstvector">The first vector to interpolate from.</param>
    /// <param name="secondVector">The second vector to interpolate to.</param>
    /// <param name="weight">The weight of the interpolation, between 0 and 1.</param>
    /// <returns>A new vector that is the result of the interpolation.</returns>
    public static Vector3 LerpVector3(Vector3 firstvector, Vector3 secondVector, float weight) =>
        new Vector3(Mathf.Lerp(firstvector.X, secondVector.X, weight), Mathf.Lerp(firstvector.Y, secondVector.Y, weight), Mathf.Lerp(firstvector.Z, secondVector.Z, weight));
}