using Gambetto.Scripts.GameCore.Grid;
using Gambetto.Scripts.GameCore.Piece;
using UnityEngine;

namespace Gambetto.Scripts.GameCore
{
    public class PowerUp
    {
        private Color personalizedDark = new Color(24f / 255f, 22f / 255f, 22f / 255f);

        //type of powerUp
        public PieceType Type;

        //boolean to check if powerUp has been used
        public bool IsUsed { get; private set; }

        //actual powerUp object
        private GameObject _powerUpObject;

        public GameObject PowerUpObject
        {
            get => _powerUpObject;
            set => _powerUpObject = value;
        }

        //cell where powerUp is located

        public Cell PowerUpCell { get; set; }

        public PowerUp(PieceType type, GameObject powerUpObject, Cell powerUpCell)
        {
            this.Type = type;
            this._powerUpObject = powerUpObject;
            this.PowerUpCell = powerUpCell;
        }

        public void SetActive()
        {
            IsUsed = true;
            _powerUpObject.GetComponentInChildren<SpriteRenderer>().color = personalizedDark;
        }

        public void SetInactive()
        {
            IsUsed = false;
            _powerUpObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        }
    }
}
