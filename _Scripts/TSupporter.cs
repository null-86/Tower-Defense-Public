using UnityEngine;

public abstract class TSupporter : Tower, ITargeter {
    public abstract void GetTarget();

    public void OnEnter(Collider2D other) {
        throw new System.NotImplementedException();
    }

    public void OnExit(Collider2D other) {
        throw new System.NotImplementedException();
    }

    public void OnHasTarget() {
        throw new System.NotImplementedException();
    }

    public void OnNoTarget() {
        throw new System.NotImplementedException();
    }

    public void OnStay(Collider2D other) {
        throw new System.NotImplementedException();
    }

    public void ResetAim() {
        throw new System.NotImplementedException();
    }

    public abstract void TakeAim();

    Unit ITargeter.GetTarget() {
        throw new System.NotImplementedException();
    }
}
