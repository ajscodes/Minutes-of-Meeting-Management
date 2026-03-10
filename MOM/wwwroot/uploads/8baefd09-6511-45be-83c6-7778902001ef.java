public class HappyNumber {
    public static void main(String[] args) {
        int n = 50;
        int result = n;
         
        while(result != 1 && result != 4){
            result = sumOfEle(result);
        }

        if(result == 1){
            System.out.println("Happy");
        }
        else{
            System.out.println("NOT");
        }
    }

    public static int sumOfEle(int n){
        int temp = n, sum = 0;
        while(temp != 0){
            int x = temp % 10;
            sum = sum + x * x;
            temp /= 10;
        }

        return sum;
    }
}
