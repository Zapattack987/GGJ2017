using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ItemDisplay : MonoBehaviour {

    private GameObject _itemObject;
    private Identifier _item;

    public void SetItem(Identifier item)
    {
        print("Setting item " + item);
        if (_itemObject != null)
        {
            Destroy(_itemObject);
            _itemObject = null;
        }

        _item = item;
        if (item != null)
        {
            _itemObject = (GameObject)Instantiate(item.gameObject);
            _itemObject.transform.position = transform.position;
            _itemObject.transform.parent = transform;
            _itemObject.layer = LayerMask.NameToLayer("UI");
            _itemObject.transform.localScale = new Vector3(3, 3, 3);
        }
    }
}
