# floating-character-controller

A floating capsule character controller made in Unity 3D.
여기 코드는 Very Very Valet게임의 기술 블로그의 영상을 참고해서 구현한 했습니다. 점프 부분과 Platform에 대한 구현은 아래 글을 참고 했습니다.
https://github.com/joebinns/stylised-character-controller

https://www.youtube.com/watch?v=qdskE8PJy6Q

Floating capsule 방법이 필요한 이유는 영상에도 나오지만 다음과 같은 이유들입니다.
- Ground clearness
- No friction 
- Sticking to slope
- Control over downward forces 

## Features
### 이동
- 빠른 반응의 캐릭터 이동
- 지형의 굴곡에 영향을 받지 않은 이동
- 콜라이더가 충돌하면서 갑자기 튀어 올라가는 현상 등 해결

### 점프
- 버튼을 누른 시간에 비례하는 점프
- 코요테 점프
- 점프 버퍼링
- 점프 상태 이벤트 콜백

## 구현이 안된 부분
픽셀아트 게임 x2에서 사용하기 위해 구현을 한 프로젝트라 몇가지는 구현에서 제외 했습니다.
빠진 부분은 다음과 같습니다.
- Capsule upright updating 
- Downward forces 
- Transform forwarding

빠진 구현은, 영상을 참고해서 구현하거나, 위 링크의 구현체를 참고 해주세요.
