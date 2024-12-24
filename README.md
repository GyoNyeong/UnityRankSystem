# UnityRankSystem




<br>

## 🗂️ **프로젝트 개요**
MySql, TCPServer(C++로 작성), Unity클라이언트 간 통신을 해 랭킹 시스템을 구현
<br>

---
<br>

## 작업 내용
  1. MySql - PlayerName과 Point를 키로 하는 데이터테이블 작성

  2. TCPServer -https://github.com/GyoNyeong/RankServer 참고

  3. Unity클라이언트

     3.1 서버와의 연결

       - 서버와 연결을 담당하는 TCPClient class 생성

       - Json을 이용해 서버와 데이터 통신

       - 유니티 메인쓰레드와 분리된 서버에서 데이터를 받기만 하는 전용 쓰레드(ReceiveThred) 생성

     3.2 데이터 관리

       - 데이터를 관리하는 전용 class (RankDataQue) 생성

       - 클라이언트에서 RankDataQue class에 저장된 데이터를 이용. UI업데이트
       
