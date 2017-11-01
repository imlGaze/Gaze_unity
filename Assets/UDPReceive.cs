using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using OpenCvSharp;

public class UDPReceive : MonoBehaviour {

    int leftX;
    int leftY;
    int rightX;
    int rightY;


    int LOCA_LPORT = 31416;
    static UdpClient udp;
    Thread thread;
    Point[] pupils;
    Vector3[] unity;
    GameObject leftTopCross;
    GameObject rightTopCross;
    GameObject rightBottomCross;
    GameObject leftBottomCross;
    GameObject focus;
    GameObject gaze;
    GameObject log;
    int target = 0;
    Point leftPupil, rightPupil;

    Mat h;
    bool calcMode = true;

    // Use this for initialization
    Vector3 t = new Vector3(0, 0.1f, 0);
    void Start () {
   
        udp = new UdpClient(LOCA_LPORT);
        udp.Client.ReceiveTimeout = 1000;
        thread = new Thread(new ThreadStart(ThreadMethod));
        thread.Start();
        pupils = new Point[4];
        unity = new Vector3[4];
        h = new Mat(3, 3, MatType.CV_32FC1);
        leftTopCross = GameObject.Find("LeftTop");
        rightTopCross = GameObject.Find("RightTop");
        rightBottomCross = GameObject.Find("RightBottom");
        leftBottomCross = GameObject.Find("LeftBottom");
        focus = GameObject.Find("Focus");
        gaze = GameObject.Find("Gaze");
        log = GameObject.Find("Log");

        unity[0] = leftTopCross.transform.position;
        unity[1] = rightTopCross.transform.position;
        unity[2] = rightBottomCross.transform.position;
        unity[3] = leftBottomCross.transform.position;

        leftPupil = new Point(0, 0, 0);
        rightPupil = new Point(0, 0, 0);
        for (int i=0; i<4; i++)
        {
            pupils[i] = new Point(0, 0, 0);
        }
    }

    // Update is called once per frame
	void Update (){
        if (Input.GetKeyUp(KeyCode.C))
        {
            calcMode = !calcMode;
        }

        if (calcMode)
        {

            if (Input.GetKey(KeyCode.Space))
            {
                // leftTopCross.transform.position = new Vector3(leftX, leftY, 0);
                pupils[target] = leftPupil;
            }
            else if (Input.GetKey(KeyCode.Return))
            {
                float[] pp = new float[]
                {
                    pupils[0].x, pupils[0].y,
                    pupils[1].x, pupils[1].y,
                    pupils[2].x, pupils[2].y,
                    pupils[3].x, pupils[3].y,
                };
                float[] up = new float[]
                {
                    unity[0].x, unity[0].y,
                    unity[1].x, unity[1].y,
                    unity[2].x, unity[2].y,
                    unity[3].x, unity[3].y,
                };

                Mat p = new Mat(4, 2, MatType.CV_64FC1, pp);
                Mat s = new Mat(4, 2, MatType.CV_64FC1, up);

                h = Cv2.FindHomography(p, s);
                Debug.Log(h);
            }
            else if (Input.GetKey(KeyCode.Alpha1))
            {
                target = 0;
                focus.transform.position = unity[target] + t;
            }
            else if (Input.GetKey(KeyCode.Alpha2))
            {
                target = 1;
                focus.transform.position = unity[target] + t;
            }
            else if (Input.GetKey(KeyCode.Alpha3))
            {
                target = 2;
                focus.transform.position = unity[target] + t;
            }
            else if (Input.GetKey(KeyCode.Alpha4))
            {
                target = 3;
                focus.transform.position = unity[target] + t;
            }
            log.GetComponent<TextMesh>().text = "LOOK";
        }
        else
        {
            Mat lp = new Mat(3, 1, MatType.CV_64FC1, new float[] {
                leftPupil.x, leftPupil.y, 1,
            });

            Mat g = h * lp;
            gaze.transform.position = new Vector3(g.At<float>(0), g.At<float>(1), 0.3f);
            string str = g.At<float>(0) + ", " + g.At<float>(1);
            log.GetComponent<TextMesh>().text = str;
        }
    }


    void OnApplicationQuit()
    {
        thread.Abort();        
    }

    void ThreadMethod()
    {
        while (true)
        {
            IPEndPoint remoteEP = null;
            byte[] data = udp.Receive(ref remoteEP);


            leftX = ((data[0] << 24) | (data[1] << 16) | (data[2] << 8) | (data[3] << 0));
            leftY = ((data[4] << 24) | (data[5] << 16) | (data[6] << 8) | (data[7] << 0));
            rightX = ((data[8] << 24) | (data[9] << 16) | (data[10] << 8) | (data[11] << 0));
            rightY = ((data[12] << 24) | (data[13] << 16) | (data[14] << 8) | (data[15] << 0));
            Debug.Log("[" + leftX + "," + leftY + "], [" + rightX + ", " + rightY + "]");

            leftPupil = new Point(leftX, leftY, 0);
            rightPupil = new Point(rightX, rightY, 0);
        }
    }
}
