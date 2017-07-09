using UnityEngine;
using System.Collections;

public class MTuple<T,G>{

    public T v1;
    public G v2;
    public MTuple(T t, G g)
    {
        v1 = t;
        v2 = g;
    }

}
