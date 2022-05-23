import React from 'react';
import { useState } from "react";

export default function DataLoader(){
  const [files, setFiles] = useState([])
  const [message, setMessage] = useState(null)
  const [isLoading, setLoading] = useState(false)

  const onFileChange = (e) => {
    setFiles(e.target.files)
  }

  const onFileUpload = async (e) =>{  
    e.preventDefault()  
    const formData = new FormData();
    for (let file of files){
      formData.append("files", file)
    }
    setLoading(true)
    try{
      const resp = await fetch("api/v1/weather", {
        method: 'POST',
        body: formData
      })      
      if(resp.ok){     
        setMessage("success")
      } else{
        let result = await resp.json()
        setMessage(`error: ${result.message}`)
      }
    } catch(err){
      setMessage("internal error")
    } finally{
      setLoading(false)
    }
    
  }
    return (
        <div>
            <h1>
              Загрузка файлов
            </h1>
            <form onSubmit={(e) => onFileUpload(e)}>
                <input type="file" accept="application/vnd.ms-excel, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" onChange={onFileChange} multiple/>
                <input disabled={!files.length} type="submit" value="Upload" />
            </form>
            {isLoading ? <span>Loading is in progress...</span>: <></>}
            <span>{message}</span>
        </div>
      );
}