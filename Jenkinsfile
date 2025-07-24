pipeline {
    agent any

    environment {
        DOTNET_CLI_HOME = '/tmp/.dotnet'
        APP_NAME = 'YourAppName' // Thay bằng tên DLL chính của bạn
        PORT = '81'
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Restore') {
            steps {
                sh 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build --configuration Release --no-restore'
            }
        }

        stage('Test') {
            steps {
                sh 'dotnet test --no-build --no-restore'
            }
        }

        stage('Publish') {
            steps {
                sh """
                dotnet publish \
                --configuration Release \
                --output ./publish \
                --no-build \
                --no-restore
                """
            }
        }

        stage('Deploy') {
            steps {
                script {
                    // Dừng ứng dụng nếu đang chạy
                    sh(script: "pkill -f dotnet || true", returnStatus: true)
                    
                    // Triển khai ứng dụng
                    sh """
                    cd publish
                    nohup dotnet ${APP_NAME}.dll --urls http://*:${PORT} > app.log 2>&1 &
                    """
                    
                    // Kiểm tra ứng dụng đã chạy chưa
                    def appRunning = sh(
                        script: "pgrep -f 'dotnet ${APP_NAME}.dll'",
                        returnStatus: true
                    ) == 0
                    
                    if (!appRunning) {
                        error('Failed to start application')
                    }
                }
            }
        }
    }

    post {
        success {
            echo "✅ Deployment successful! Application running on port ${PORT}"
            echo "Access at: http://${env.JENKINS_URL.split(':')[0]}:${PORT}"
        }
        failure {
            echo "❌ Deployment failed"
        }
    }
}