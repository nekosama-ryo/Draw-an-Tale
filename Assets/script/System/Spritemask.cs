using UnityEngine;

public class Spritemask : MonoBehaviour
{
    public SpriteMask _mask=default;
    [SerializeField, Header("マスクのぶれる範囲")]
    private float _min=default, _max=default;
    float _time = default;
    [SerializeField,Header("最初の大きさ")]
    float _default = 0.985f;

    public void Randomalpha()
    {
        if (_time > 2)
        {
            _default = Random.Range(_min, _max);
            _time = 0;
        }
        else
        {
            _mask.alphaCutoff = Mathf.SmoothStep(_mask.alphaCutoff, _default, _time / 2);
        }
        _time += Time.deltaTime;
    }
}
//0.985f, 0.995f
