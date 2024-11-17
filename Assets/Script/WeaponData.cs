using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���⿡ ����
public class WeaponData : MonoBehaviour
{
    public WeaponInfo weaponInfo; //  public�� �̾��� �ν����Ϳ��� ���̴±���
    Coroutine weaponCoroutine;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // �浹�� ����� Layer�� Player���...
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // �Ʒ� �ڵ��  transform. �� �ƴ϶� gameobject. �ص� ������.
            // �� ������ ���� ������Ʈ�� Transform�� ������ �־ 
            PlayerFire pf = other.transform.GetComponent<PlayerFire>();
            // �÷��̾��� ������ ���� ��ġ�� ���� ������Ų��.(���� ���� ��ü�� �θ� �������� ����)
            // �θ� - �ڽ� ���踦 ������ ��, ���� ��ü�� Transform�� �����Ͽ� �θ� �����Ѵ�.
            // ������ �ڽ� �������� �θ�
            //  ��, ���� ��ü�� �ٸ� ��ü�� ���ӵǵ��� ����� ���� ����մϴ�.
            // ���� ��ü�� �θ�-�ڽ� ���迡�� ���ӵǴ� ����� Transform�� ���ؼ��� �̷�����ϴ�. 
            // transform.parent <= �̰� ��ü�� ���� ��ü�� �θ� ��ü�� ����Ű�� �Ӽ��Դϴ�.
            transform.parent = pf.sockets[(int)weaponInfo.weaponType].transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.GetComponent<BoxCollider>().enabled = false; 
            pf.myWeapon = weaponInfo; // �� �ڵ� ������ 10�� �߿� 5���� ���� ��� 5���� ���� �ִ�.(weaponInfo�� ������ �ν����Ϳ�  public�̴ϱ�)
            pf.GetWeapon();
            // - �ڽ��� ������ �ڽ� ������Ʈ�� ����ϰ�
            // - ���� �������� (0, 0, 0)���� �����.
            // - �ڽ��� �ڽ� ������Ʈ�� ��Ȱ��ȭ �Ѵ�.
            // - ���� ������ �÷��̾�� �����Ѵ�.

        }
    }

    public void DropWeapon(WeaponInfo currentInfo)
    {
        // �÷��̾��� ���Ͽ��� ���� �����.
        // - �θ�� ��ϵ� ������Ʈ�� ���� ������ ó���Ѵ�.
        // - �ڽ��� �ڽ� ������Ʈ�� Ȱ��ȭ�Ѵ�.
        // - �÷��̾��� ���� ������ �޾ƿ´�.
        transform.parent = null;
        if(weaponCoroutine == null)
        {
            weaponCoroutine = StartCoroutine(TriggerOn(3));
        }
        transform.eulerAngles = Vector3.zero; // ���� ������ ���� �ݱ� �� �ִ� �� ó�� �ڿ������� �ϱ� ���ؼ�
        weaponInfo = currentInfo;
    }

    IEnumerator TriggerOn(float time)
    {
        yield return new WaitForSeconds(time);
        transform.GetComponent<BoxCollider>().enabled = true;
        weaponCoroutine = null;
    }
}
