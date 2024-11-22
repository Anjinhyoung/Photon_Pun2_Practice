public interface IInteractionInterface
{
    /// <summary>
    /// 데미지 기능 구현 강제
    /// </summary>
    /// <param name="dmg">실제 공격력</param>
    /// <param name="veiwID">공격자의 photon veiw ID</param>
    void TakeDamage(float dmg, int veiwID);
}
