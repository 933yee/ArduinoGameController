using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public Slider volumnSlider;
    // Start is called before the first frame update
    void Start()
    {
        if(!PlayerPrefs.HasKey("musicVolumn")){
            PlayerPrefs.SetFloat("musicVolumn", 1);
            Load();
        }else{
            Load();
        }
    }
    // Update is called once per frame
    public void ChangerVolumn()
    {
        AudioListener.volume = volumnSlider.value;
        Save();
    }
    void Load(){
        volumnSlider.value = PlayerPrefs.GetFloat("musicVolumn");
    }
    void Save(){
        PlayerPrefs.SetFloat("musicVolumn", volumnSlider.value);
    }
}
