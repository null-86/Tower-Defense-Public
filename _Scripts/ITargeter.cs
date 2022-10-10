using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargeter {
    Unit GetTarget();
    void TakeAim();

    void ResetAim();

    void OnNoTarget();

    void OnHasTarget();

    void OnEnter(Collider2D other);

    void OnStay(Collider2D other);

    void OnExit(Collider2D other);

}
