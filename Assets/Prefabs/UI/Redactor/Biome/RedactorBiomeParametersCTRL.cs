using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RedactorBiomeParametersCTRL : MonoBehaviour
{
    [SerializeField]
    SliderCTRL coofPolus;
    [SerializeField]
    SliderCTRL coofZeroX;
    [SerializeField]
    SliderCTRL coofHeight;
    [SerializeField]
    SliderCTRL coofHeightMax;
    [SerializeField]
    SliderCTRL coofHeightMin;

    private void OnEnable()
    {
        if (!RedactorBiomeCTRL.main)
            return;

        updateAll();
    }

    void updateAll() {
        updatePolus();
        updateZeroX();
        updateHeight();
        updateHeightMax();
        updateHeightMin();
    }

    void updatePolus() {
        coofPolus.slider.value = RedactorBiomeCTRL.main.biomeData.coofPolus * 100;
        coofPolus.slider.minValue = -50;
        coofPolus.slider.maxValue = 50;
        coofPolus.SetValueText(RedactorBiomeCTRL.main.biomeData.coofPolus.ToString());
    }
    void updateZeroX() {
        coofZeroX.slider.value = RedactorBiomeCTRL.main.biomeData.coofZeroX * 100;
        coofZeroX.slider.minValue = -50;
        coofZeroX.slider.maxValue = 50;
        coofZeroX.SetValueText(RedactorBiomeCTRL.main.biomeData.coofZeroX.ToString());
    }
    void updateHeight() {
        coofHeight.slider.value = RedactorBiomeCTRL.main.biomeData.coofHeight * 100;
        coofHeight.slider.minValue = -50;
        coofHeight.slider.maxValue = 50;
        coofHeight.SetValueText(RedactorBiomeCTRL.main.biomeData.coofHeight.ToString());
    }
    void updateHeightMax() {
        coofHeightMax.slider.value = RedactorBiomeCTRL.main.biomeData.coofHeightMax;
        coofHeightMax.slider.minValue = 0;
        coofHeightMax.slider.maxValue = 100;
        coofHeightMax.SetValueText();
    }
    void updateHeightMin() {
        coofHeightMin.slider.value = RedactorBiomeCTRL.main.biomeData.coofHeightMin;
        coofHeightMin.slider.maxValue = 0;
        coofHeightMin.slider.minValue = 100;
        coofHeightMin.SetValueText();
    }

    public void AcceptCoofPolus() {
        coofPolus.slider.minValue = -50;
        coofPolus.slider.maxValue = 50;

        //�������� �������� � �������� �� -1 �� 1

        float result = coofPolus.slider.value * 0.01f;
        coofPolus.SetValueText(result.ToString());

        BiomeData biome = RedactorBiomeCTRL.main.biomeData;
        biome.coofPolus = result;
        RedactorBiomeCTRL.SetBiome(biome);
    }
    public void AcceptCoofZeroX() {
        coofZeroX.slider.minValue = -50;
        coofZeroX.slider.maxValue = 50;

        //�������� �������� � �������� �� -1 �� 1
        float result = coofZeroX.slider.value * 0.01f;
        coofZeroX.SetValueText(result.ToString());

        BiomeData biome = RedactorBiomeCTRL.main.biomeData;
        biome.coofZeroX = result;
        RedactorBiomeCTRL.SetBiome(biome);
    }
    public void AcceptCoofHeight() {
        coofHeight.slider.minValue = -50;
        coofHeight.slider.maxValue = 50;

        //�������� �������� � �������� �� -1 �� 1
        float result = coofHeight.slider.value * 0.01f;
        coofHeight.SetValueText(result.ToString());

        BiomeData biome = RedactorBiomeCTRL.main.biomeData;
        biome.coofHeight = result;
        RedactorBiomeCTRL.SetBiome(biome);
    }
    public void AcceptCoofHeightMin() {
        coofHeightMin.slider.minValue = 0;
        coofHeightMin.slider.maxValue = 100;

        //���� ������� ������ ���������
        if (coofHeightMin.slider.value > coofHeightMax.slider.value)
        {
            //�������� ���������� ����� �� ��� �������
            coofHeightMax.slider.value = coofHeightMin.slider.value;
            AcceptCoofHeightMax();
        }



        //�������� �������� � �������� �� 0 �� 100
        float result = coofHeightMin.slider.value;
        coofHeightMin.SetValueText();

        BiomeData biome = RedactorBiomeCTRL.main.biomeData;
        biome.coofHeightMin = result;
        RedactorBiomeCTRL.SetBiome(biome);
    }
    public void AcceptCoofHeightMax() {
        coofHeightMax.slider.minValue = 0;
        coofHeightMax.slider.maxValue = 100;

        //���� �������� ������ ��������
        if (coofHeightMax.slider.value < coofHeightMin.slider.value) {
            //������� ���������� ����� �� ��� ��������
            coofHeightMin.slider.value = coofHeightMax.slider.value;
            AcceptCoofHeightMin();
        }

        //�������� �������� � �������� �� 0 �� 100
        float result = coofHeightMax.slider.value;
        coofHeightMax.SetValueText(result.ToString());

        BiomeData biome = RedactorBiomeCTRL.main.biomeData;
        biome.coofHeightMax = result;
        RedactorBiomeCTRL.SetBiome(biome);
    }
}
