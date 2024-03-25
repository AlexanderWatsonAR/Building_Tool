using UnityEngine;

public class Dog : MonoBehaviour, IBuildable
{
	[SerializeField] DogData m_Data;

	public DogData Data => m_Data;

	public IBuildable Initialize (IData data)
	{
		m_Data = data as DogData;
		return this;
	}
	public void Build ()
	{
		
	}
	public void Demolish ()
	{
		
	}
}