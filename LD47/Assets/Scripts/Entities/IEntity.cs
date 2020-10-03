public interface IEntity
{

    float Health { get; set; }

    void Damage(float amount);

    void OnUpdateHealth();

}