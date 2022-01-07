using ScriptableObjects;

[System.Serializable]
public class FloatReference {
    public bool UseConstant = true;

    public float ConstantValue;
    public FloatValue Variable;

    public float Value {
        get => UseConstant ? ConstantValue : Variable.value;
        set {
            if (UseConstant) 
                ConstantValue = value;
            else 
                Variable.value = value;
        }
    }
}
