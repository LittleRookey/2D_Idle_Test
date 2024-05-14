using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BigNumber{

	// 소수점 계산은 하지 않습니다. 무조건 정수형.
	// 10억 (1000^3) 이상 차이가 나는 계산은 무시 합니다.
	// 계산은 본 클래스의 레퍼런스 밸류를 통해 이뤄져야 합니다.

	private string myFirstNum;
	private string mySecondNum;
	private string myThirdNum;
	private string myDigits;

	public BigNumber(){
		myFirstNum = "0";
		mySecondNum = "";
		myThirdNum = "";
		myDigits = "";
	}

	public BigNumber(string setString){
		CalcStringToBigNum (setString);
	}

	public BigNumber(int firstNum,int secondNum, int thirdNum,string strDigits) : this(firstNum.ToString(),secondNum.ToString(),thirdNum.ToString(),strDigits){
		// Constructor chaining.
	}

	public BigNumber(string firstNum,string secondNum,string thirdNum,string strDigits){
		myFirstNum = firstNum;
		mySecondNum = secondNum;
		myThirdNum = thirdNum;
		myDigits = strDigits;
	}

	public BigNumber(BigNumber setBigNumber){
		myFirstNum = setBigNumber.GetFirstNum();
		mySecondNum = setBigNumber.GetSecondNum();
		myThirdNum = setBigNumber.GetThirdNum ();
		myDigits = setBigNumber.GetDigits ();
	}

	public string GetDigits(){
		return myDigits;
	}

	public string GetFirstNum(){
		return myFirstNum;
	}

	public string GetSecondNum(){
		return mySecondNum;
	}

	public string GetThirdNum(){
		return myThirdNum;
	}

	public void CalcStringToBigNum(string setString){
		int stringCount = setString.Length;
		int valueDigit = (stringCount % 3 == 0) ? stringCount / 3 - 1 : stringCount / 3;
		// 자릿수 계산.
		// 0~3자릿수의 수 (0 ~ 999)는 0이됨. (none)
		// 4~6 (1000 ~ 999999) 수는 1이됨 (A)
		// 7~9 (1000000 ~ 999999999) 수는 2가됨 (B)
		string firstNum,secondNum,thirdNum,digits;

		if (stringCount > 6) {
			int cutIndex = (stringCount - 1) % 3 + 1;
			// 0 (none) 포함 이므로, -1 해주고 자릿수 계산
			// 0 이면 한 자리, 1 이면 두 자리, 2 이면 세 자리.
			firstNum = setString.Substring (0, cutIndex);
			secondNum = setString.Substring (cutIndex, 3);
			thirdNum = setString.Substring (cutIndex + 3, 3);
			digits = CalcStringDigit (valueDigit);
		}
		else if (stringCount > 3) {
			int cutIndex = (stringCount - 1) % 3 + 1;
			// 0 (none) 포함 이므로, -1 해주고 자릿수 계산
			// 0 이면 한 자리, 1 이면 두 자리, 2 이면 세 자리.
			firstNum = setString.Substring (0, cutIndex);
			secondNum = setString.Substring (cutIndex, 3);
			thirdNum = "";
			digits = CalcStringDigit (valueDigit);
		} else {
			firstNum = setString;
			secondNum = "";
			thirdNum = "";
			digits = "";
		}

		myFirstNum = firstNum;
		mySecondNum = secondNum;
		myThirdNum = thirdNum;

		myDigits = digits;
	}

	public string CalcStringDigit(int valueDigit){
		// 자릿수를 받아서 영문자로 환산.
		// (A ~ Z) 65 ~ 90

		if (valueDigit == 0) {
			return "";
		}

		string returnDigit = "";

		int temp = valueDigit;

		// A~Z , AA ~ ZZ , AAA ~ ZZZ 등등, 몇개의 문자열이 추가되어야 하는가. 26^n 꼴임.

		Stack<char> stringDigitStack= new Stack<char>();

		while (true) {
			stringDigitStack.Push (Convert.ToChar (65 + (temp - 1) % 26));

			if (temp % 26 != 0) {
				temp /= 26;
			} else {
				temp /= 26;
				temp--;
			}

			if (temp <= 0) {
				break;
			}
		}

		while (stringDigitStack.Count > 0) {
			returnDigit += stringDigitStack.Pop ();
		}

		return returnDigit;
	}

	public int CalcDigitToInt(string digits){
		Stack<char> stringDigitStack = new Stack<char> ();
		for (int i = 0; i < digits.Length; i++) {
			stringDigitStack.Push (digits [i]);
		}

		int returnValue = 0;
		float power = 0f;

		while (stringDigitStack.Count > 0) {
			returnValue += (Convert.ToInt32 (stringDigitStack.Pop ()) - 64) * (int)Mathf.Pow (26, power);
			power++;
		}

		return returnValue;
	}

	public string GetNum(){
		if (mySecondNum != "") {
			return myFirstNum + "." + mySecondNum + myDigits;
		} else {
			return myFirstNum + myDigits;
		}
	}

	public static bool operator >(BigNumber compare1, BigNumber compare2){
		if (compare1.GetDigits () == compare2.GetDigits ()) {
			if (compare1.GetFirstNum () == compare2.GetFirstNum ()) {
				if (compare1.GetSecondNum () == compare2.GetSecondNum ()) {
					if (compare1.GetThirdNum () == compare2.GetThirdNum ()) {
						return false; // 수가 같음.
					} else {
						return (System.Convert.ToInt16 (compare1.GetThirdNum ()) > System.Convert.ToInt16 (compare2.GetThirdNum ()));
					}
				} else {
					return (System.Convert.ToInt16 (compare1.GetSecondNum ()) > System.Convert.ToInt16 (compare2.GetSecondNum ()));
				}
			} else {
				return (System.Convert.ToInt16 (compare1.GetFirstNum ()) > System.Convert.ToInt16 (compare2.GetFirstNum ()));
			}
		} else {
			if (compare1.GetDigits ().Length != compare2.GetDigits ().Length) {
				return (compare1.GetDigits ().Length > compare2.GetDigits ().Length);
			} else {
				for (int i = 0; i < compare1.GetDigits ().Length ; i++) {
					if (compare1.GetDigits () [i] != compare2.GetDigits () [i]) {
						int compare1ToInt = System.Convert.ToInt32 ((char)compare1.GetDigits () [i]);
						int compare2ToInt = System.Convert.ToInt32 ((char)compare2.GetDigits () [i]);

						return compare1ToInt > compare2ToInt;
					}
				}

				// 완전히 digit 이 같은 경우에 return false 가 콜되는데, 사실 여기는 맨 처음 if 문, compare1과 compare2 가 같은가에서 이미 빠져버리기 때문에 유명 무실함.
				// 하지만 이 구문이 없으면 코드중 일부만 값을 반환한다고 떠서 형식상 넣음.
				return false;
			}
		}
	}

	public static bool operator <(BigNumber compare1,BigNumber compare2){
		if (compare1.GetDigits () == compare2.GetDigits ()) {
			if (compare1.GetFirstNum () == compare2.GetFirstNum ()) {
				if (compare1.GetSecondNum () == compare2.GetSecondNum ()) {
					if (compare1.GetThirdNum () == compare2.GetThirdNum ()) {
						return false; // 수가 같음.
					}
				}
			}
		}

		return !(compare1 > compare2);
	}

	public static BigNumber operator +(BigNumber add1,BigNumber add2){
		int digitDistance = add1.GetDigitDistance (add2);
		if (digitDistance == -1) {
			// 유효 계산 범위 초과의 차이.
			if (add1 > add2) {
				return add1;
			} else {
				return add2;
			}
		} else {
			// 실 계산.

			BigNumber bigOne,smallOne;

			if (add1 > add2) {
				bigOne = add1;
				smallOne = add2;
			} else {
				bigOne = add2;
				smallOne = add1;
			}

			int bigOneValue, smallOneValue;

			bigOneValue = System.Convert.ToInt32 (bigOne.GetFirstNum () + bigOne.GetSecondNum () + bigOne.GetThirdNum ());
			smallOneValue = System.Convert.ToInt32 (smallOne.GetFirstNum () + smallOne.GetSecondNum () + smallOne.GetThirdNum ());

			if (smallOne.GetSecondNum () != "" && smallOne.GetThirdNum () != "") {
				smallOneValue /= (int)Mathf.Pow (1000f, digitDistance - 1f);
			}

			string returnValue = (bigOneValue + smallOneValue).ToString();
			int addDigits = 0;
			string firstTemp = "", secondTemp = "", thirdTemp = "";
			if (returnValue.Length > 9) {
				// 자릿수가 추가 된 경우.
				addDigits = Mathf.CeilToInt ((returnValue.Length - 9) / 3);
				firstTemp = returnValue.Substring (0, 3 - returnValue.Length % 3);
				secondTemp = returnValue.Substring (3 - returnValue.Length % 3, 6 - returnValue.Length % 3);
				thirdTemp = returnValue.Substring (6 - returnValue.Length % 3, 9 - returnValue.Length % 3);
			} else {
				if (returnValue.Length <= 3) {
					firstTemp = returnValue;
					secondTemp = "";
					thirdTemp = "";
				} else if (returnValue.Length <= 6) {
					secondTemp = returnValue.Substring (returnValue.Length - 3, 3);
					firstTemp = returnValue.Substring (0, returnValue.Length - 3);
				} else if (returnValue.Length <= 9) {
					thirdTemp = returnValue.Substring (returnValue.Length - 3, 3);
					secondTemp = returnValue.Substring (returnValue.Length - 6, 3);
					firstTemp = returnValue.Substring (0, returnValue.Length - 6);
				}
			}

			BigNumber resultBigNumber = new BigNumber (bigOne);
			string updateDigits = resultBigNumber.CalcStringDigit (resultBigNumber.CalcDigitToInt (resultBigNumber.GetDigits ()) + addDigits);
			resultBigNumber.myDigits = updateDigits;
			resultBigNumber.myFirstNum = firstTemp;
			resultBigNumber.mySecondNum = secondTemp;
			resultBigNumber.myThirdNum = thirdTemp;

			return resultBigNumber;
		}
	}

	public static BigNumber operator -(BigNumber minusOne,BigNumber minusTwo){
		bool isBig;

		if (minusOne > minusTwo) {
			isBig = true;
		} else {
			isBig = false;
		}

		int digitDistance = minusOne.GetDigitDistance (minusTwo);

		if (!isBig) {
			// 빼지는 수가 더 작다면, (결과값이 음수가 나올 상황)
			return new BigNumber("0"); // 음수 구현 시러.
		}

		if (digitDistance == -1) {
			return minusOne;
		} else {
			// 이하는 결과 값이 무조건 양수일 때임.

			// 음수 구현 원한다면 빅넘버 클래스에 bool로 음수 양수 판별 따로 넣어주던지.
			int bigOneValue, smallOneValue;

			bigOneValue = System.Convert.ToInt32 (minusOne.GetFirstNum () + minusOne.GetSecondNum () + minusOne.GetThirdNum ());
			smallOneValue = System.Convert.ToInt32 (minusTwo.GetFirstNum () + minusTwo.GetSecondNum () + minusTwo.GetThirdNum ());

			if (minusTwo.GetSecondNum () != "" && minusTwo.GetThirdNum () != "") {
				smallOneValue /= (int)Mathf.Pow (1000f, digitDistance - 1f);
			}

			string returnValue = (bigOneValue - smallOneValue).ToString();
			int addDigits = 0;
			string firstTemp = "", secondTemp = "", thirdTemp = "";
			if (returnValue.Length > 9) {
				// 자릿수가 추가 된 경우.
				addDigits = Mathf.CeilToInt ((returnValue.Length - 9) / 3);
				firstTemp = returnValue.Substring (0, 3 - returnValue.Length % 3);
				secondTemp = returnValue.Substring (3 - returnValue.Length % 3, 6 - returnValue.Length % 3);
				thirdTemp = returnValue.Substring (6 - returnValue.Length % 3, 9 - returnValue.Length % 3);
			} else {
				if (returnValue.Length <= 3) {
					firstTemp = returnValue;
					secondTemp = "";
					thirdTemp = "";
				} else if (returnValue.Length <= 6) {
					secondTemp = returnValue.Substring (returnValue.Length - 3, 3);
					firstTemp = returnValue.Substring (0, returnValue.Length - 3);
				} else if (returnValue.Length <= 9) {
					thirdTemp = returnValue.Substring (returnValue.Length - 3, 3);
					secondTemp = returnValue.Substring (returnValue.Length - 6, 3);
					firstTemp = returnValue.Substring (0, returnValue.Length - 6);
				}
			}

			BigNumber resultBigNumber = new BigNumber (minusOne);
			string updateDigits = resultBigNumber.CalcStringDigit (resultBigNumber.CalcDigitToInt (resultBigNumber.GetDigits ()) + addDigits);
			resultBigNumber.myDigits = updateDigits;
			resultBigNumber.myFirstNum = firstTemp;
			resultBigNumber.mySecondNum = secondTemp;
			resultBigNumber.myThirdNum = thirdTemp;

			return resultBigNumber;
		}

	}

	public static BigNumber operator *(BigNumber multiplyOne,BigNumber multiplyTwo){
		BigNumber bigOne, smallOne;

		if (multiplyOne > multiplyTwo) {
			bigOne = multiplyOne;
			smallOne = multiplyTwo;
		} else {
			bigOne = multiplyTwo;
			smallOne = multiplyOne;
		}

		string bigOneStr, smallOneStr;
		bigOneStr = bigOne.GetFirstNum () + bigOne.GetSecondNum () + bigOne.GetThirdNum ();
		smallOneStr = smallOne.GetFirstNum () + smallOne.GetSecondNum () + smallOne.GetThirdNum ();

		int bigOneFirst, smallOneFirst;
		int bigOneLength, smallOneLength;

		bigOneLength = bigOneStr.Length;
		smallOneLength = smallOneStr.Length;
		if (bigOneStr.Length <= 4) {
			bigOneFirst = Convert.ToInt32(bigOneStr);
			smallOneFirst = Convert.ToInt32(smallOneStr);

			bigOneLength = 0;
			smallOneLength = 0;
		} else {
			bigOneFirst = Convert.ToInt32(bigOneStr.Substring (0, 4));
			bigOneLength = bigOne.GetFirstNum().Length + bigOne.CalcDigitToInt (bigOne.GetDigits ()) * 3 - 4; // big one의 계산부를 제외한 뒤의 0개수. (자릿수)
			if (smallOneStr.Length <= 4) {
				smallOneFirst = Convert.ToInt32(smallOneStr);
				smallOneLength = 0;
			} else {
				smallOneFirst = Convert.ToInt32(smallOneStr.Substring (0, 4));
				smallOneLength = smallOne.GetFirstNum().Length + smallOne.CalcDigitToInt (smallOne.GetDigits ()) * 3 - 4; // small one 의 뒤 0개수.
			}
		}

		if (bigOneLength < 0) {
			bigOneLength = 0;
		}

		if (smallOneLength < 0) {
			smallOneLength = 0;
		}
		string calcResult = (bigOneFirst * smallOneFirst).ToString();
		// 이하 코딩.

		int resultDigit = bigOneLength + smallOneLength + calcResult.Length;

		BigNumber resultBigNumber = new BigNumber (bigOne);
		int peek = (resultDigit % 3 == 0) ? 3 : resultDigit % 3;
		resultBigNumber.myFirstNum = calcResult.Substring (0, peek);
		resultBigNumber.mySecondNum = "";
		resultBigNumber.myThirdNum = "";

		if (calcResult.Length > peek) {
			resultBigNumber.mySecondNum = calcResult.Substring (peek, (calcResult.Length - peek < 3) ? calcResult.Length - peek : 3);
		}

		if (calcResult.Length > peek + 3) {
			resultBigNumber.myThirdNum = calcResult.Substring (peek + 3, (calcResult.Length - peek - 3 < 3) ? calcResult.Length - peek - 3 : 3);
		}

		bool resultDigitIsDividedByThree = false;

		if (resultDigit % 3 == 0) {
			resultDigitIsDividedByThree = true;
		}

		resultDigit = resultDigit / 3;

		if (resultDigitIsDividedByThree) {
			resultDigit--;
		}

		if (resultDigit < 0) {
			resultDigit = 0;
		}


		resultBigNumber.myDigits = resultBigNumber.CalcStringDigit (resultDigit);
		return resultBigNumber;
	}

	public static BigNumber operator /(BigNumber divideOne,BigNumber divideTwo){
		if (divideTwo > divideOne) {
			return new BigNumber ("0");
		}

		if (divideTwo < new BigNumber ("1")) {
			return divideOne;
		}

		int oneDexDigits = divideOne.GetFirstNum ().Length + divideOne.CalcDigitToInt (divideOne.GetDigits ()) * 3; // 10 자릿수 개수. (ex : 5000 -> 4 (자리))
		int twoDexDigits = divideTwo.GetFirstNum ().Length + divideTwo.CalcDigitToInt (divideTwo.GetDigits ()) * 3;
		int dexDigitDistance = oneDexDigits - twoDexDigits;
		int lastAddDex = dexDigitDistance - 8;
		lastAddDex = (lastAddDex <= 0) ? 0 : lastAddDex; // 둘의 자릿수 차이가 8보다 크다면, twoDexDigits 를 1의 자리로 연산후에 그 결과값에 다시 10^lastAddDex 만큼 곱해줘야 함.

		float divideOneFloat = (float) Convert.ToInt32 (divideOne.GetFirstNum () + divideOne.GetSecondNum () + divideOne.GetThirdNum ());
		string divideTwoTemp = divideTwo.GetFirstNum () + divideTwo.GetSecondNum () + divideTwo.GetThirdNum ();

		float divideTwoFloat = Convert.ToInt32 (divideTwoTemp);

		int divideCount = 0;

		while (divideTwoFloat >= 10f) {
			divideTwoFloat /= 10f;
			divideCount++;
		}

		float resultNum = divideOneFloat / divideTwoFloat;

		int oneDigitExceed = divideOne.CalcDigitToInt (divideOne.GetDigits ()) - 2;
		int twoDigitExceed = divideTwo.CalcDigitToInt (divideTwo.GetDigits ()) - 2;
		oneDigitExceed = (oneDigitExceed < 0) ? 0 : oneDigitExceed;
		twoDigitExceed = (twoDigitExceed < 0) ? 0 : twoDigitExceed;

		int resultDexDigits = ((int)resultNum).ToString ().Length - divideCount + oneDigitExceed * 3 - twoDigitExceed * 3;

		while (resultNum < 100000000f) {
			resultNum *= 10f;
		}

		string resultNumStr = Mathf.FloorToInt (resultNum).ToString ();

		int cutIndex = resultDexDigits % 3;
		cutIndex = (cutIndex == 0) ? 3 : cutIndex;

		if (resultDexDigits == 0) {
			cutIndex = 1;
		}

		BigNumber resultBigNumber = new BigNumber ();
		if (resultDexDigits > 6) {
			resultBigNumber.myFirstNum = resultNumStr.Substring (0, cutIndex);
			resultBigNumber.mySecondNum = resultNumStr.Substring (cutIndex, 3);
			resultBigNumber.myThirdNum = resultNumStr.Substring (cutIndex + 3, 3);
		} else if (resultDexDigits > 3) {
			resultBigNumber.myFirstNum = resultNumStr.Substring (0, cutIndex);
			resultBigNumber.mySecondNum = resultNumStr.Substring (cutIndex, 3);
			resultBigNumber.myThirdNum = "";
		} else {
			resultBigNumber.myFirstNum = resultNumStr.Substring (0, cutIndex);
			resultBigNumber.mySecondNum = "";
			resultBigNumber.myThirdNum = "";
		}

		if (resultDexDigits > 0) {
			resultBigNumber.myDigits = resultBigNumber.CalcStringDigit ((resultDexDigits - 1) / 3);
		}


		return resultBigNumber;
	}

	public int GetDigitDistance(BigNumber otherValue){
		// -5 : 버그 코드. (원래는 안 나오는게 정상임. 예외처리 삐끗한게 있는거다.);
		// -1 : 계산 무시 가능 범위. 1000^3 이상의 차이.

		// 3 : 차이가 1000^2 ~ 1000^3 - 1 범위. (third num 으로 계산하여야 함.) (자릿수 차이가 2).
		// 2 : 차이가 1000 ~ 1000^2 -1 범위. (second num + third num 으로 계산하여야 함.) (자릿수 차이가 1).
		// 1 : 차이가 0 ~ 999 범위. (first num + second num + third num 으로 계산하여야 함.) (완벽히 같은 자릿수)

		int myDigitsInt, otherDigitsInt;

		myDigitsInt = CalcDigitToInt (GetDigits ());
		otherDigitsInt = CalcDigitToInt (otherValue.GetDigits ());

		int returnDistance = Mathf.Abs (myDigitsInt - otherDigitsInt);

		if (returnDistance > 2) {
			return -1;
		} else {
			return returnDistance + 1;
		}

		/*if (Mathf.Abs (otherValue.GetDigits ().Length - myDigits.Length) >= 2) {
			// 자릿수 개수 차이가 2 이상 (예 : A , AAA) 일 경우 묻고 따질 것도 없이 계산 무시 반환.
			return -1;
		} else if (Mathf.Abs (otherValue.GetDigits ().Length - myDigits.Length) == 1) {
			// 자릿수 개수 차이가 1 이면, 작은수의 맨 뒷자리와, 큰 수의 맨 뒷자리를 기반으로 케이스를 나눠서 따져야 함.
			// 단, 큰 수의 맨 뒷자리를 제외한 모든 자릿수는 A 여야하며, 동시에 작은 수의 맨 뒷자리를 제외한 모든 자릿수는 다 Z 여야 함.
			// (EX : AAAAB, ZZZZ) -> 3 반환.
			BigNumber biggerClass, smallerClass;

			if (GetDigits ().Length > otherValue.GetDigits ().Length) {
				biggerClass = this;
				smallerClass = otherValue;
			} else {
				biggerClass = otherValue;
				smallerClass = this;
			}

			for (int i = 1; i < biggerClass.GetDigits ().Length - 1; i++) {
				if (biggerClass.GetDigits () [i].Equals ('A')) {
					// 앞 글자들이 A 가 아닌 경우 계산 범위 초과.
					return -1;
				}
			}

			for (int i = 1; i < smallerClass.GetDigits ().Length - 1; i++) {
				if (smallerClass.GetDigits () [i].Equals ('Z')) {
					// 앞 글자들이 Z가 아닌 경우 계산 범위 초과.
					return -1;
				}
			}

			char lastBiggerDigits = biggerClass.GetDigits () [biggerClass.GetDigits ().Length - 1];
			char lastSmallerDigits;

			if (smallerClass.GetDigits () == "") {
				lastSmallerDigits = '@'; // 비어 있다는 표시.
			} else {
				lastSmallerDigits = smallerClass.GetDigits () [smallerClass.GetDigits ().Length - 1];
			}

			if (lastBiggerDigits.Equals ('A')) {
				if (lastSmallerDigits.Equals ('Z') || lastSmallerDigits.Equals('@')) {
					return 2;
				} else if (lastSmallerDigits.Equals ('X')) {
					return 3;
				} else {
					// 계산 범위 초과.
					return -1;
				}
			} else if (lastBiggerDigits.Equals ('B')) {
				if (lastSmallerDigits.Equals ('Z') || lastSmallerDigits.Equals('@')) {
					return 3;
				} else {
					return -1;
				}
			} else {
				return -1;
			}
		} else if (GetDigits ().Length == otherValue.GetDigits ().Length) {
			if (GetDigits ().Length == 0) {
				// 둘다 0~999 사이 이므로.
				return 1;
			}

			// 자릿수 개수가 똑같으면 영단어 -> 자릿수 반환으로 간격 계산.
			if (GetDigits ().Length >= 2) {
				for (int i = 0; i < GetDigits ().Length - 2; i++) {
					// 맨 뒤 자릿수 + 맨 뒤에서 하나 앞 제외, 앞 자릿수 중에 하나라도 이쪽이 더 크다면, 1000^26 이상 큰 것이므로 계산 제외.
					if (GetDigits () [i] != otherValue.GetDigits () [i]) {
						return -1;
					}
				}

				char compare1Char, compare2Char;
				compare1Char = GetDigits () [GetDigits ().Length - 2];
				compare2Char = otherValue.GetDigits () [otherValue.GetDigits ().Length - 2];

				char biggerChar, smallerChar;

				if (Mathf.Abs (System.Convert.ToInt32 (compare1Char) - System.Convert.ToInt32 (compare2Char)) == 1) {
					// BZ 와 CA 같은 경우의 자릿수 크기 차이 반환.

					if (System.Convert.ToInt32 (compare1Char) == System.Convert.ToInt32 (compare2Char) + 1) {
						// 내가 상대보다 더 큰 수 일경우.
						biggerChar = GetDigits () [GetDigits ().Length - 1];
						smallerChar = otherValue.GetDigits () [otherValue.GetDigits ().Length - 1];
					} else {
						biggerChar = otherValue.GetDigits () [otherValue.GetDigits ().Length - 1];
						smallerChar = GetDigits () [GetDigits ().Length - 1];
					}

					if (biggerChar.Equals ('A')) {
						if (smallerChar.Equals ('Z')) {
							return 2;
						} else if (smallerChar.Equals ('X')) {
							return 3;
						} else {
							return -1;
						}
					} else if (biggerChar.Equals ('B')) {
						if (smallerChar.Equals ('Z')) {
							return 2;
						} else {
							return -1;
						}
					} else {
						return -1;
					}
				}
			}

			char myLastDigits, otherLastDigit;

			myLastDigits = GetDigits () [GetDigits ().Length - 1];
			otherLastDigit = otherValue.GetDigits () [otherValue.GetDigits ().Length - 1];

			int myCharCode, otherCharCode;

			myCharCode = System.Convert.ToInt32 ((char)myLastDigits);
			otherCharCode = System.Convert.ToInt32 ((char)otherLastDigit);

			switch (Mathf.Abs (myCharCode - otherCharCode)) {
			case 0:
			case 1:
			case 2:
				return Mathf.Abs (myCharCode - otherCharCode) + 1;

			default:
				return -1;
			}
		} else {
			// 여기 까지 내려 올 일이 없음.
			// 버그 코드. -5.
			return -5;
		}*/
	}
}
