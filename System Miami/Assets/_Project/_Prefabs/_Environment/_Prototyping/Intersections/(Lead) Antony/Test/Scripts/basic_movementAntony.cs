using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    public class basic_movementAntony : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        
        Rigidbody2D rb;
        Vector2 movementDir;
        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {
            movementDir.x = Input.GetAxis("Horizontal");
            movementDir.y = Input.GetAxis("Vertical");
        }

        private void FixedUpdate()
        {
            rb.MovePosition(rb.position + movementDir * speed * Time.fixedDeltaTime);
        }
    }
}
