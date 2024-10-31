using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ShopSystemPackage
{

    public class PlayerShopVer3 : MonoBehaviour
    {
        [SerializeField] SpriteRenderer playerImage;
        [SerializeField] TMP_Text playerNameText;

        public float speed = 1f;

        Rigidbody2D rb;
        bool isMoving = false;
        float x, y;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            ChangePlayerSkin();
        }
        private void ChangePlayerSkin()
        {
            CharacterShopVer3 character = GameDataManager.GetSelectedCharacter();

            if (character.image != null)
            {
                playerImage.sprite = character.image;
                playerNameText.text = character.name;

            }
        }
        private void Update()
        {
            x = Input.GetAxis("Horizontal");
            y = Input.GetAxisRaw("Vertical");

            isMoving = (x != 0f || y != 0f);
        }

        private void FixedUpdate()
        {
            if (isMoving)
            {
                rb.position += new Vector2(x, y) * speed * Time.fixedDeltaTime;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            string tag = other.collider.tag;

            if (tag.Equals("coin"))
            {
                // Add coins
                GameDataManager.AddCoins(32);

                // Cheating (while moving hold key "C" to get extra coins)
#if UNITY_EDITOR
                if (Input.GetKey(KeyCode.C))
                {
                    GameDataManager.AddCoins(179);
                }
#endif
                GameSharedUI.Instance.UpdateCoinsUIText();
                Destroy(other.gameObject);
            }
        }
    }

}