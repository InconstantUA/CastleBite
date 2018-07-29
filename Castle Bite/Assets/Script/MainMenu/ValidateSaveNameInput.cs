using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValidateSaveNameInput : MonoBehaviour
{
    [SerializeField]
    Color normalColor;
    [SerializeField]
    Color errorColor;
    public InputField mainInputField;
    [SerializeField]
    int maxLength;

    IEnumerator ActivateInputField()
    {
        yield return null;
        mainInputField.Select();
    }

    void OnEnable()
    {
        // reset fields
        // Clear text
        mainInputField.text = "";
        // reset color for placeholder to normal
        mainInputField.transform.Find("Placeholder").GetComponent<Text>().color = normalColor;
        // Activate input field on next frame
        StartCoroutine(ActivateInputField());
    }

    public void Start()
    {
        // Sets the MyValidate method to invoke after the input field's default input validation invoke (default validation happens every time a character is entered into the text field.)
        mainInputField.onValidateInput += delegate (string input, int charIndex, char addedChar) { return MyValidate(addedChar); };
    }

    private char MyValidate(char charToValidate)
    {
        // check if size limit is not reached
        // limit is 25 characters
        if (mainInputField.text.Length > maxLength)
        {
            // change it to an empty character.
            charToValidate = '\0';
        }
        return charToValidate;
    }

    void CheckForEmtpyString()
    {
        // verify if input text is set
        if (mainInputField.text != "")
        {
            // highlight string with normal color
            mainInputField.transform.Find("Placeholder").GetComponent<Text>().color = normalColor;
        }
        else
        {
            // highlight string with red color
            mainInputField.transform.Find("Placeholder").GetComponent<Text>().color = errorColor;
        }
    }

    public void ValidateOnEditEnd()
    {
        CheckForEmtpyString();
    }

    public void ValidateOnValueChanged()
    {
        CheckForEmtpyString();
    }
}