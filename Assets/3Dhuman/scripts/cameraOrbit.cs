using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraOrbit : MonoBehaviour
{
    protected Transform _XForm_Camera;
    protected Transform _XForm_Parent;
    protected Vector3 _LocalRotation;
    protected float _CameraDistance = 10f;
    public float MouseSentivity = 4f;
    public float ScrollSentivity = 2f;
    public float OrbitDampening = 10f;
    public float ScrollDampening = 6f;
    public bool CameraDisabled = false;
    public Transform player;
    public float zoomSpeed, cameraSpeed;
    public int ZoomState = 0, CameraStateX = 0, CameraStateY = 0;
    // Start is called before the first frame update
    void Start()
    {
        this._XForm_Camera = this.transform;
        this._XForm_Parent = this.transform.parent;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        _XForm_Parent.position = player.position + new Vector3(0, 3f, 0);
        if (!CameraDisabled)
        {
            // Debug.Log(Input.GetAxis("Mouse X") * MouseSentivity);
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                _LocalRotation.x += Input.GetAxis("Mouse X") * MouseSentivity;
                _LocalRotation.y -= Input.GetAxis("Mouse Y") * MouseSentivity;
                _LocalRotation.y = Mathf.Clamp(_LocalRotation.y, 0f, 90f);
            }
            if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                float ScrollAmount = Input.GetAxis("Mouse ScrollWheel") * ScrollSentivity;
                // Debug.Log(ScrollAmount);
                ScrollAmount *= this._CameraDistance * 0.3f;
                this._CameraDistance += ScrollAmount * -1f;
                this._CameraDistance = Mathf.Clamp(this._CameraDistance, 1.5f, 100f);
            }
            cameraMove();
            zoom();
        }
        Quaternion QT = Quaternion.Euler(_LocalRotation.y, _LocalRotation.x, 0);
        this._XForm_Parent.rotation = Quaternion.Lerp(this._XForm_Parent.rotation, QT, Time.deltaTime * OrbitDampening);
        if (this._XForm_Camera.localPosition.z != this._CameraDistance * -1f)
        {
            this._XForm_Camera.localPosition = new Vector3(0f, 0f, Mathf.Lerp(this._XForm_Camera.localPosition.z, this._CameraDistance * -1f, Time.deltaTime * ScrollDampening));
        }
    }
    void zoom()
    {
        float ScrollAmount = ZoomState * zoomSpeed;
        ScrollAmount *= this._CameraDistance * 0.3f;
        this._CameraDistance += ScrollAmount * -1f;
        this._CameraDistance = Mathf.Clamp(this._CameraDistance, 1.5f, 100f);
    }

    void cameraMove()
    {
        _LocalRotation.x += cameraSpeed * CameraStateX;
        _LocalRotation.y -= cameraSpeed * CameraStateY;
        _LocalRotation.y = Mathf.Clamp(_LocalRotation.y, 0f, 90f);
    }

}
