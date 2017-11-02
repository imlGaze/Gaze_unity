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
	GameObject leftPupilObj;
	GameObject rightPupilObj;
	int target = 0;
    Point leftPupil, rightPupil;

    Mat h = new Mat(3, 3, MatType.CV_64FC1, new double[] { 0,0,0, 0,0,0, 0,0,0 });
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
        leftTopCross = GameObject.Find("LeftTop");
        rightTopCross = GameObject.Find("RightTop");
        rightBottomCross = GameObject.Find("RightBottom");
        leftBottomCross = GameObject.Find("LeftBottom");
        focus = GameObject.Find("Focus");
        gaze = GameObject.Find("Gaze");
		log = GameObject.Find("Log");
		leftPupilObj = GameObject.Find("LeftPupil");
		rightPupilObj = GameObject.Find("RightPupil");

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

        if (Input.GetKey(KeyCode.Space))
        {
            // leftTopCross.transform.position = new Vector3(leftX, leftY, 0);
            pupils[target] = leftPupil;
			leftPupilObj.GetComponent<TextMesh>().text = leftPupil.x + ", " + leftPupil.y;
			rightPupilObj.GetComponent<TextMesh>().text = rightPupil.x + ", " + rightPupil.y;
		}
		else if (Input.GetKey(KeyCode.Return))
        {
            double[] pp = new double[]
            {
                pupils[0].x, pupils[0].y,
                pupils[1].x, pupils[1].y,
                pupils[2].x, pupils[2].y,
                pupils[3].x, pupils[3].y,
            };
            double[] up = new double[]
            {
                unity[0].x, unity[0].y,
                unity[1].x, unity[1].y,
                unity[2].x, unity[2].y,
                unity[3].x, unity[3].y,
            };

			Debug.LogWarning(pp);
			Debug.LogWarning(up);

			Mat p = new Mat(4, 2, MatType.CV_64FC1, pp);
            Mat s = new Mat(4, 2, MatType.CV_64FC1, up);
            Mat hm = Cv2.FindHomography(p, s);
			if (hm.Size().Width > 0)
			{
				h = hm;
				Debug.LogWarning("OK");
			}
			else
			{
				Debug.LogWarning("Bad Points");
			}
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

		string str = "";
		for (int i=0; i<9; i++)
		{
			int c = i % 3;
			int r = i / 3;

			str += h.At<double>(r, c) + ", " + (c == 2 ? "\n" : "");
		}

		log.GetComponent<TextMesh>().text = str;

		if (! calcMode)
		{
			Mat lp = new Mat(3, 1, MatType.CV_64FC1, new double[] {
                leftPupil.x, leftPupil.y, 1
            });

            Mat g = h * lp;
            gaze.transform.position = new Vector3((float) g.At<double>(0, 0), (float) g.At<double>(1, 0), 0.3f);
            str = g.At<double>(0, 0) + ", " + g.At<double>(1, 0);
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
