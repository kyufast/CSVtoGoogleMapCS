using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVtoGoogleMapCS
{
    public class Person
    {
        public enum MovingStatus   : int
        {
            ontrain, stopping, walking, others, start
        };

        MovingStatus nowStatus;

        public Person()
        {
            nowStatus = MovingStatus.start;    
        }

        public void setStatusOnTrain(){
            nowStatus = MovingStatus.ontrain;
        }
        public void setStatusStopping()
        {
            nowStatus = MovingStatus.stopping;
        }
        public void setStatusWalking()
        {
            nowStatus = MovingStatus.walking;
        }
        public void setStatusOthers()
        {
            nowStatus = MovingStatus.others;
        }
        public MovingStatus getStatus()
        {
            return nowStatus;
        }

    }
}
