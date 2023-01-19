using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZCU.TechnologyLab.Common.Serialization.Properties;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Serializers;

public class ColiderProperty : OptionalProperty
{
    /// <summary> Property name </summary>
    string propertyName = "BoxColiderSize";
    
    ArraySerializer<float> floatSerializer;

    public void Start()
    {
        floatSerializer = new ArraySerializer<float>(propertyName, sizeof(float));
    }

    /// <summary>
    /// Getter for property name
    /// </summary>
    /// <returns> Property name </returns>
    public override string GetPropertyName()
    {
        return propertyName;
    }

    /// <summary>
    /// Processes incoming Colider center positon and size property, called when object is recieved from the server
    /// - recieves properties dictionary, and if it contains the color property then sets the property onto the game object
    /// </summary>
    /// <param name="properties"> Properties dictionary </param>
    public override void Process(Dictionary<string, byte[]> properties)
    {
        if (!properties.ContainsKey(propertyName))
            return;

        // Deserialize color
        floatSerializer = new ArraySerializer<float>(propertyName, sizeof(float));
        float[] vals = floatSerializer.Deserialize(properties);
        Vector3[] vecs = ConvertorHelper.FloatsToVec3(vals);

        // Set color to mesh renderer
        var cldr = GetComponent<BoxCollider>();
        cldr.center = vecs[0];
        cldr.size = vecs[1];
    }

    /// <summary>
    /// Serializes the center position and size property, called when object is sent to the server
    /// - gets the value of the property and returns serialized byte array value
    /// </summary>
    /// <returns> Byte array with value of the property </returns>
    public override byte[] Serialize()
    {
        // Get color
        var cldr = GetComponent<BoxCollider>();
        float[] vals = ConvertorHelper.Vec3ToFloats(new Vector3[] { cldr.center , cldr.size });

        // Serialize color
        byte[] serialized = floatSerializer.Serialize(vals);
        return serialized;
    }
}
