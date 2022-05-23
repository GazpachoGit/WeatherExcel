import React from 'react';
import { useState, useEffect, useRef } from "react";
import { useHistory } from 'react-router-dom';

export default function Weather(){
    const [filters, setFilters] = useState({month: "", year:"", page: 1})
    const [months, setMonths] = useState([])
    const [years, setYears] = useState([])
    const [totalFound, setTotalFound] = useState(0)
    const [pageCount, setPageCount] = useState([])
    const [weatherList, setWeatherList] = useState([])
    const history = useHistory();
    const didMountRef = useRef(false);
    const pageSize = useRef(25)
    const [errorMessage, setErrorMessage] = useState(null)
   
    useEffect( () => {
        const query = new URLSearchParams(history.location.search)
        const currentMonth = query.get('month')
        const currentYear = query.get('year')
        const currentPage = query.get('page')
        const getLayout = async () => {
            const resp = await fetch("api/v1/weather/layout", {
                method: 'GET'
            })           
            if(resp.ok){
                setErrorMessage(null)
                let result = await resp.json()
                setFilters({
                    month: currentMonth? currentMonth : result.months[0],
                    year: currentYear ? currentYear : result.years[0],
                    page: currentPage ? currentPage : 1,
                })
                setMonths(result.months)
                setYears(result.years)
            } else{
                setErrorMessage("load layout error")
            }
        }
        getLayout()
    }, [history]);

    useEffect(() => {
        if(didMountRef.current){
            const query = (new URLSearchParams({month: filters.month, year: filters.year, page: filters.page})).toString()
            history.replace({search: query})
            const getData  = async () => {
                const resp = await fetch(`api/v1/weather/${history.location.search}`, {
                    method: 'GET'
                })                
                if(resp.ok){
                    setErrorMessage(null)
                    let result = await resp.json()
                    setTotalFound(result.totalFound)
                    setWeatherList(result.result)
                    const count = Math.ceil(result.totalFound / pageSize.current)
                    if(count > 0){
                        const pageNumArray = []
                        for(let i = 1; i < count; i++){
                            pageNumArray.push(i)
                        }
                        setPageCount(pageNumArray)
                    } else{
                        setPageCount([])
                    }                                                          
                } else {
                    setErrorMessage("load weather error")
                }
            }
            getData()
        } else{
            didMountRef.current = true
        }               
    }, [filters, history])
    const monthChangeHandler = (e) =>{
        setFilters({...filters, month: e.target.value})
    }
    const yearChangeHandler = (e) =>{
        setFilters({...filters, year: e.target.value})
    }
    const setPageHandler = (n) => {
        setFilters({...filters, page: n})
    }

    return (
        <>
        {(!errorMessage) ? <>
            <div>
        <select value={filters.month} onChange={monthChangeHandler}>
            {months.map(m => <option key={m} value={m}>{m}</option>)}
        </select>
        <select value={filters.year} onChange={yearChangeHandler}>
            {years.map(m => <option key={m} value={m}>{m}</option>)}
        </select>
        </div>
        <span>totalFound: {totalFound}</span>
        <div className='weather-page-container'>
            {pageCount.map(n => <span key={n} className='weather-page-link' onClick={e => setPageHandler(n)}>{n}</span>)}
        </div>
        <div>
        <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Year</th>
            <th>Month</th>
            <th>Date</th>
            <th>Time</th>
            <th>Temperature</th>
            <th>Dew Point</th>
            <th>Humidity</th>
            <th>Pressure</th>
            <th>Visibility</th>
            <th>Wind Direction</th>
            <th>Wind Speed</th>
          </tr>
        </thead>
        <tbody>
          {weatherList.map(forecast =>
            <tr key={forecast.id}>
              <td>{forecast.year}</td>
              <td>{forecast.month}</td>
              <td>{forecast.date}</td>
              <td>{forecast.time}</td>
              <td>{forecast.temperature}</td>
              <td>{forecast.dewPoint}</td>
              <td>{forecast.humidity}</td>
              <td>{forecast.pressure}</td>
              <td>{forecast.visibility}</td>
              <td>{forecast.windDirection}</td>
              <td>{forecast.windSpeed}</td>
            </tr>
          )}
        </tbody>
      </table>
        </div>
            </> : <span>{errorMessage}</span>
        }
        </>
                      
    )
}