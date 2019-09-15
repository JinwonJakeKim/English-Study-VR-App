using System;
using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class AICharacterControl : MonoBehaviour
    {
        public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
        public ThirdPersonCharacter character { get; private set; } // the character we are controlling
        public Transform target, target2;                                    // target to aim for
        private int target_flag, changed;

        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();

            agent.updateRotation = false;
            agent.updatePosition = true;

            changed = 1;//target이 초기에 설정되거나 바뀌었을 때만 setDestination을 해서 효율을 높이기위한 flag 변수
            target_flag = 1;
        }


        private void Update()
        {
            if (changed == 1)
            {   //target을 2개까지 지정할 수 있으며, flag에 따라 캐릭터의 target이 설정된다.
                if (target != null && target2 != null)
                {//target 두개가 다 설정이 되있어야해
                    if (target_flag == 1) agent.SetDestination(target.position);//초기에는 target이 1로 설정되어있어 target1로 간다.
                    else if (target_flag == 2) agent.SetDestination(target2.position);
                }
            }
            //target에 지정해 놓은 거리와 현재 위치간 남은거리가 있을 경우 캐릭터는 움직인다. 
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                character.Move(agent.desiredVelocity, false, false);
                changed = 0;
            }
            else
            {
                //character.Move(Vector3.zero, false, false);//아니면 캐릭터는 멈춘다.
                if (target_flag == 1) { target_flag = 2; changed = 1; }
                else if (target_flag == 2) { target_flag = 1; changed = 1; }
            }
        }
        public void SetTarget(Transform target)
        {
            this.target = target;
        }
    }
}
