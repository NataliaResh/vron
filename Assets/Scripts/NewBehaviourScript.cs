using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Slide : MonoBehaviour
{
    [SerializeField] private float _minGroundNormalY = .65f;
    [SerializeField] private float _gravityModifier = 1f;
    [SerializeField] private Vector2 _velocity; //óñêîðåíèå ãðàâèòàöèîííîå
    [SerializeField] private LayerMask _layerMask; //ôèëüòð äëÿ îáúåêòîâ
    [SerializeField] private float _speed; //ìîäèôèêòîð ñêîðîñòè

    private Rigidbody2D _rb2d;
    private SpriteRenderer _sprite;

    private Vector2 _groundNormal; // íîðìàëü ê ïîâåõíîñòè
    private Vector2 _targetVelocity;    //èòîãîâîå óñêîðåíèå âäîëü ïîâåðõíîñòè
    private bool _grounded; //ïðîâåðêà ÷òî ñòîèì íà çåìëå
    private ContactFilter2D _contactFilter;  //íàñòðîéêà ôèëüòðîâ
    private RaycastHit2D[] _hitBuffer = new RaycastHit2D[16]; //ìàññèâ îáúåêòîâ, ñ êîðîòûìè ìîæåì ñòîëêíóòüñÿ 
    private List<RaycastHit2D> _hitBufferList = new List<RaycastHit2D>(16); //äëÿ óäîáñòâà ìàññèâ ïåðåâîäèì â ëèñò

    private const float MinMoveDistance = 0.001f;
    private const float ShellRadius = 0.01f;

    private void OnEnable()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _contactFilter.useTriggers = false;
        _contactFilter.SetLayerMask(_layerMask);
        _contactFilter.useLayerMask = true;
    }

    private void Update()
    {
        Vector2 alongSurface = Vector2.Perpendicular(_groundNormal); //íàïðàâëåíèå äâèæåíèÿ âäîëü ïîâåðõíîñòè

        _targetVelocity = alongSurface * _speed;  //ñêîðîñòü äâèæåíèÿ âäîëü ïîâåðõíîñòè
    }

    private void FixedUpdate()
    {
        _velocity += _gravityModifier * Physics2D.gravity * Time.deltaTime;
        _velocity.x = _targetVelocity.x;

        _grounded = false;

        Vector2 deltaPosition = _velocity * Time.deltaTime; //äëèíà âîçìîæíîãî øàãà çà 1 êàäð
        Vector2 moveAlongGround = new Vector2(_groundNormal.y, -_groundNormal.x); //íàïðàâëåíèå âäîëü ïîâåðõíîñòè
        Vector2 move;

        if (_groundNormal.x >= 0)
        {
            _sprite.flipX = false;
            move = moveAlongGround * (-deltaPosition.x); //äëèíà âîçìîæíîãî øàãà âäîëü ïîâåðõíîñòè çà 1 êàäð òîëüêî ïî Õ
        }
        else
        {
            _sprite.flipX = true;
            move = moveAlongGround * (deltaPosition.x); //äëèíà âîçìîæíîãî øàãà âäîëü ïîâåðõíîñòè çà 1 êàäð òîëüêî ïî Õ

        }

        Movement(move, false); //ñíà÷àëà äâèãàåìñÿ ïî Õ

        move = Vector2.up * deltaPosition.y; // ïåðåçàïèñûâàåì âåêòîð, ÷òîáû ïîëó÷èòü íàïðàâëåíèå äâèæåíèÿ ïî Y
        Movement(move, true); // äâèãàåìñÿ ïî Y

        if (Input.GetKey(KeyCode.Space))
        {
            StartCoroutine(Jump(_groundNormal));
        }
    }

    private void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude; //îïðåäåëåÿåì äëèíó øàãà

        if (distance > MinMoveDistance) //èãíîðèðóåì øàãè íåçíà÷èòåëüíîé äëèíû
        {

            int count = //êîëè÷åñòâî îáúåêòîâ ñ êåì ìû ìîæåì ñòîëêíóòüñÿ
                _rb2d.Cast( //ïðîåöèðóåì íàø êîëëàéäåð ïî õîäó äâèæåíèÿ
                move, _contactFilter,  //óêàçûâàåì îáúåêòû, ñ êåì ìû õîòèì ñòîëêíóòüñÿ
                _hitBuffer, // ïîëó÷àåì âñå îáúåêòû, ñ êîòîðûìè íàì ïðåäñòîèò ñòîëêíóòüñÿ
                distance +
                ShellRadius);  //äîïîëíèòåëüíàÿ çîíà âîêðóã ïåðñîíàæà, êîòîðàÿ ïîçâîëÿåò íåìíîãî óâåëè÷èòü êîëëàéäåð ïåðñîíàæà

            _hitBufferList.Clear();

            for (int i = 0; i < count; i++)
            {
                _hitBufferList.Add(_hitBuffer[i]);
            }

            for (int i = 0; i < _hitBufferList.Count; i++) //ïåðåáèðàåì âñå îáúåêòû, ñ êîòîðûìè ìîæåì ñòîëêíóòüñÿ
            {
                Vector2 currentNormal = _hitBufferList[i].normal;  //îïðåäåëÿåì íîðìàëü ïîâåðõíîñòè, íà êîòîðóþ ñîáèðàåìñÿ âñòàòü
                if (currentNormal.y > _minGroundNormalY) //ñìîòèì ÷òî Y íîðìàëè áîëüøå ìèíèìàëüíî âîçìîæíîé íîðìàëè
                {
                    _grounded = true;
                    if (yMovement)
                    {
                        _groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(_velocity, currentNormal); //ñêàëÿðíîå ïðîèçâåäåíèå âåêòîðîâ
                if (projection < 0)
                {
                    _velocity = _velocity - projection * currentNormal;
                }

                float modifiedDistance = _hitBufferList[i].distance - ShellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance; //âûñ÷èòûâàåì ìèíèìàëüíîå ðàññòîÿíèå, íà êîòîðîå ìîæåì øàãíóòü
            }
        }

        _rb2d.position = _rb2d.position + move.normalized * distance; //èòîãîâîå èçìåíåíèå ïîçèöèè
    }

    private IEnumerator Jump(Vector2 groundNormal)
    {
        if (_grounded)
        {
            float directionX = groundNormal.x == 0 ? 1 : groundNormal.x / Mathf.Abs(groundNormal.x);
            float y = 0.3f, x = 0.05f * directionX;
            int countPoints = 30;
            float deltaY = y / countPoints;


            for (int i = 0; i < countPoints; i++)
            {
                _rb2d.position = _rb2d.position + new Vector2(x, y);
                y -= deltaY;
                yield return new WaitForSeconds(0.01f);
            }

            for (int i = 0; i < countPoints; i++)
            {
                _rb2d.position = _rb2d.position + new Vector2(x, y);
                yield return new WaitForSeconds(0.01f);
            }

        }
    }
}