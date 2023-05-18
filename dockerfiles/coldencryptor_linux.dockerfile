FROM mono
WORKDIR /coldencryptor
ADD https://raw.githubusercontent.com/2XXE-SRA/payload_resources/master/coldencryptor/coldencryptor_linux.cs /coldencryptor/coldencryptor.cs
RUN csc -out:/coldencryptor/coldencryptor /coldencryptor/coldencryptor.cs
ENTRYPOINT ["mono", "/coldencryptor/coldencryptor"] 
