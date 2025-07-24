pipeline {
    agent any

    environment {
        DOTNET_CLI_HOME = '/tmp/.dotnet'
        APP_NAME = 'cicd' // Tên file DLL không có đuôi .dll
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
                bat 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                bat 'dotnet build --configuration Release --no-restore'
            }
        }

        stage('Test') {
            steps {
                bat 'dotnet test --no-build --no-restore'
            }
        }

        stage('Publish') {
            steps {
                bat """
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
                    bat "taskkill /F /IM ${APP_NAME}.exe || exit 0"

                    // Triển khai ứng dụng
                    bat """
                    cd publish
                    start /B dotnet ${APP_NAME}.dll --urls http://*:${PORT}
                    """
                    
                    // Kiểm tra ứng dụng đã chạy chưa
                    def appRunning = bat(
                        script: "tasklist | findstr /I '${APP_NAME}.exe'",
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
            echo "Access at: http://your-server-ip:${PORT}"
        }
        failure {
            echo "❌ Deployment failed"
        }
    }
}